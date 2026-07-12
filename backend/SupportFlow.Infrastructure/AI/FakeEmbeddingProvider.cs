using SupportFlow.Application.AI.Interfaces;

namespace SupportFlow.Infrastructure.AI;

public class FakeEmbeddingProvider : IEmbeddingProvider
{
    public Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var embedding = new float[1536];

        embedding[0] = text.Length;
        embedding[1] = text.Count(char.IsLetter);
        embedding[2] = text.Count(char.IsDigit);

        return Task.FromResult(embedding);
    }
}

