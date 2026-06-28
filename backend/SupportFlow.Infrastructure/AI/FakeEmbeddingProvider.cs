using SupportFlow.Application.AI.Interfaces;

namespace SupportFlow.Infrastructure.AI;

public class FakeEmbeddingProvider : IEmbeddingProvider
{
    public Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var embedding = new float[]
        {
            text.Length,
            text.Count(char.IsLetter),
            text.Count(char.IsDigit)
        };

        return Task.FromResult(embedding);
    }
}

