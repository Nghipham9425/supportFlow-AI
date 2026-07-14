using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Tickets;
using SupportFlow.Application.Tickets.DTOs;


namespace SupportFlow.Infrastructure.AI.OpenAI;

public class OpenAITicketDraftReplyGenerator : ITicketDraftReplyGenerator
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;

    public OpenAITicketDraftReplyGenerator(HttpClient httpClient, IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    public async Task<string> GenerateDraftReplyAsync(TicketDto ticket, IReadOnlyList<RelatedKnowledgeDto> relatedKnowledge)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is missing. Set OpenAI:ApiKey with user-secrets.");
        }
        if (string.IsNullOrWhiteSpace(_options.ChatModel))
        {
            throw new InvalidOperationException(
            "OpenAI chat model is missing. Set OpenAI:ChatModel.");
        }
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var request = new OpenAIChatRequest(
            _options.ChatModel,
            [
                new OpenAIChatMessage("system", BuildSystemPrompt()),
                new OpenAIChatMessage("user", BuildUserPrompt(ticket, relatedKnowledge))
            ],
            0.3);

        var response = await _httpClient.PostAsJsonAsync(
        "https://api.openai.com/v1/chat/completions",
        request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
            $"OpenAI chat request failed with status {(int)response.StatusCode}: {errorBody}");
        }
        var result = await response.Content.ReadFromJsonAsync<OpenAIChatResponse>();

        var draftReply = result?.Choices.FirstOrDefault()?.Message.Content;

        if (string.IsNullOrWhiteSpace(draftReply))
        {
            throw new InvalidOperationException(
                "OpenAI chat response did not include a draft reply.");
        }

        return draftReply;
    }
    private static string BuildSystemPrompt()
    {
        return """
        You are a professional customer support agent for SupportFlow.
        Write clear, helpful, and concise draft replies.
        Use the provided knowledge base context when it is relevant.
        Do not invent policy details or technical steps that are not present in the ticket or context.
        If information is missing, ask the customer for the specific missing detail.
        Keep the tone friendly and professional.
        """;
    }
    private static string BuildUserPrompt(TicketDto ticket, IReadOnlyList<RelatedKnowledgeDto> relatedKnowledge)
    {
        var knowledgeContext = relatedKnowledge.Count == 0
        ? "No related knowledge base context was found."
        : string.Join(
            "\n\n",
            relatedKnowledge.Take(3).Select((item, index) => $"Context {index + 1}: {item.ArticleTitle}\n{item.Content}"));
        return $"""
        Customer name: {ticket.CustomerName}
        Customer email: {ticket.CustomerEmail}
        Subject: {ticket.Subject}
        Description:
        {ticket.Description}

        AI summary:
        {ticket.AiSummary ?? "No AI summary available."}

        Related knowledge base context:
        {knowledgeContext}

        Write a draft email reply to the customer.
        Include:
        - a greeting using the customer's name
        - acknowledgement of the issue
        - useful next steps based on the context
        - a short closing from SupportFlow Support Team
        """;
    }
}
internal sealed record OpenAIChatRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")] OpenAIChatMessage[] Messages,
    [property: JsonPropertyName("temperature")] double Temperature);

internal sealed record OpenAIChatMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);

internal sealed record OpenAIChatResponse(
    [property: JsonPropertyName("choices")] OpenAIChatChoice[] Choices);

internal sealed record OpenAIChatChoice(
    [property: JsonPropertyName("message")] OpenAIChatMessage Message);

