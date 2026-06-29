namespace SupportFlow.Infrastructure.AI.OpenAI;

public class OpenAIOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
}
