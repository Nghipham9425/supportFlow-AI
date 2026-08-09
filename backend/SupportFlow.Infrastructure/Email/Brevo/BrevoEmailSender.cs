using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlow.Application.Email;
using SupportFlow.Application.Email.Interfaces;

namespace SupportFlow.Infrastructure.Email.Brevo;

public class BrevoEmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;
    private readonly BrevoOptions _options;

    public BrevoEmailSender(
        HttpClient httpClient,
        IOptions<BrevoOptions> options
    )
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    public async Task<string?> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "Brevo API key is missing. Configure Brevo:ApiKey.");
        }
        if (string.IsNullOrWhiteSpace(_options.SenderEmail))
        {
            throw new InvalidOperationException("Brevo sender email is missing. Configure Brevo:SenderEmail.");
        }

        var requestBody = new BrevoEmailRequest(
            new BrevoSender(_options.SenderName, _options.SenderEmail),
            [new BrevoRecipient(message.RecipientEmail)],
            message.Subject,
            message.HtmlContent);

        using var request = new HttpRequestMessage(
        HttpMethod.Post,
        "https://api.brevo.com/v3/smtp/email");
            
        request.Headers.Add("api-key", _options.ApiKey);
        request.Content = JsonContent.Create(requestBody);


        using var response = await _httpClient.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(
            cancellationToken);
            
            if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Brevo email request failed with status {(int)response.StatusCode}: {responseBody}");
        }

        var result = JsonSerializer.Deserialize<BrevoEmailResponse>(
            responseBody);

        return result?.MessageId;

    }
}

internal sealed record BrevoEmailRequest(
    [property: JsonPropertyName("sender")] BrevoSender Sender,
    [property: JsonPropertyName("to")] BrevoRecipient[] To,
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("htmlContent")] string HtmlContent);

internal sealed record BrevoSender(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email);

internal sealed record BrevoRecipient(
    [property: JsonPropertyName("email")] string Email);

internal sealed record BrevoEmailResponse(
    [property: JsonPropertyName("messageId")] string? MessageId);
