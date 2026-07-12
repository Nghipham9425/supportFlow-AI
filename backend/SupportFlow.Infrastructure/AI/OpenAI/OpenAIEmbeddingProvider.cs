using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlow.Application.AI.Interfaces;

namespace SupportFlow.Infrastructure.AI.OpenAI;

public class OpenAIEmbeddingProvider : IEmbeddingProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;

    public OpenAIEmbeddingProvider(
        HttpClient httpClient,
        IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is missing. Set OpenAI:ApiKey with user-secrets.");
        }

        if (string.IsNullOrWhiteSpace(_options.EmbeddingModel))
        {
            throw new InvalidOperationException(
                "OpenAI embedding model is missing. Set OpenAI:EmbeddingModel.");
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var request = new OpenAIEmbeddingRequest(
            _options.EmbeddingModel,
            text,
            "float");

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.openai.com/v1/embeddings",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                $"OpenAI embeddings request failed with status {(int)response.StatusCode}: {errorBody}");
        }

        var result = await response.Content.ReadFromJsonAsync<OpenAIEmbeddingResponse>();

        var embedding = result?.Data.FirstOrDefault()?.Embedding;

        if (embedding is null || embedding.Length == 0)
        {
            throw new InvalidOperationException("OpenAI embeddings response did not include an embedding.");
        }

        return embedding;
    }
}

internal sealed record OpenAIEmbeddingRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("input")] string Input,
    [property: JsonPropertyName("encoding_format")] string EncodingFormat);

internal sealed record OpenAIEmbeddingResponse(
    [property: JsonPropertyName("data")] OpenAIEmbeddingData[] Data);

internal sealed record OpenAIEmbeddingData(
    [property: JsonPropertyName("embedding")] float[] Embedding);
