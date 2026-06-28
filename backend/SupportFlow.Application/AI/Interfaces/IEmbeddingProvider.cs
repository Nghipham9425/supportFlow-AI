namespace SupportFlow.Application.AI.Interfaces;

public interface IEmbeddingProvider
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}