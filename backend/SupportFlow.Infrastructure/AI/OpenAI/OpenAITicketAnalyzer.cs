using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Tickets.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using SupportFlow.Domain.Enums;

namespace SupportFlow.Infrastructure.AI.OpenAI;

public class OpenAITicketAnalyzer : ITicketAnalyzer
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;

    public OpenAITicketAnalyzer(HttpClient httpClient, IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    public async Task<TicketAnalysisResultDto> AnalyzeAsync(TicketDto ticket)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is missing. Set OpenAI:ApiKey with user-secrets."
            );
        }
        if (string.IsNullOrWhiteSpace(_options.ChatModel))
        {
            throw new InvalidOperationException(
                "OpenAI chat model is missing. Set OpenAI:ChatModel."
            );
        }
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var request = new OpenAIChatRequest(_options.ChatModel, [
            new OpenAIChatMessage("system", BuildSystemPrompt()),
            new OpenAIChatMessage("user",BuildUserPrompt(ticket))
        ],
        0.2);

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.openai.com/v1/chat/completions", request
        );
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"OpenAI chat request failed with status {(int)response.StatusCode}: {errorBody}"
            );
        }
        var result = await response.Content.ReadFromJsonAsync<OpenAIChatResponse>();

        var content = result?.Choices.FirstOrDefault()?.Message.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException(
                "OpenAI analysis response did not include content.");
        }

        return ParseAnalysisResult(content, ticket);
    }
    private static string BuildSystemPrompt()
    {
        return """
        You are a support ticket triage assistant for SupportFlow.
        Analyze the customer ticket and return only valid JSON.
        Do not include markdown, explanations, or extra text.
        """;
    }
    private static string BuildUserPrompt(TicketDto ticket)
    {
        return $@"
    Analyze this support ticket.

    Customer name: {ticket.CustomerName}
    Customer email: {ticket.CustomerEmail}
    Subject: {ticket.Subject}
    Description:
    {ticket.Description}

    Return JSON with this exact shape:
    {{
    ""summary"": ""short support summary"",
    ""category"": ""Other | Billing | TechnicalIssue | AccountAccess | ProductQuestion | BugReport"",
    ""priority"": ""Low | Medium | High | Critical"",
    ""sentiment"": ""Neutral | Confused | Frustrated | Angry""
    }}
    ";
    }

    private static TicketAnalysisResultDto ParseAnalysisResult(string content, TicketDto ticket)
{
    var cleanContent = StripCodeFences(content);

    var json = JsonSerializer.Deserialize<OpenAITicketAnalysisResponse>(
        cleanContent,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    if (json is null)
    {
        throw new InvalidOperationException("OpenAI analysis response could not be parsed.");
    }

    return new TicketAnalysisResultDto
    {
        Summary = string.IsNullOrWhiteSpace(json.Summary)
            ? $"AI analysis for {ticket.Subject}"
            : json.Summary,
        SuggestedCategory = Enum.Parse<TicketCategory>(json.Category, ignoreCase: true),
        SuggestedPriority = Enum.Parse<TicketPriority>(json.Priority, ignoreCase: true),
        Sentiment = Enum.Parse<TicketSentiment>(json.Sentiment, ignoreCase: true)
    };
}

private static string StripCodeFences(string content)
{
    var trimmed = content.Trim();

    if (trimmed.StartsWith("```"))
    {
        var firstNewLine = trimmed.IndexOf('\n');
        if (firstNewLine >= 0)
        {
            trimmed = trimmed[(firstNewLine + 1)..];
        }

        if (trimmed.EndsWith("```"))
        {
            trimmed = trimmed[..^3];
        }
    }

    return trimmed.Trim();
}

}

internal sealed record OpenAITicketAnalysisResponse(
    [property: JsonPropertyName("summary")] string Summary,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("priority")] string Priority,
    [property: JsonPropertyName("sentiment")] string Sentiment);

