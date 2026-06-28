using SupportFlow.Application.Knowledge.DTOs;

namespace SupportFlow.Application.Knowledge.Interfaces;

public interface IKnowledgeEmbeddingService
{
    Task<List<KnowledgeChunkDto>?> GenerateEmbeddingsAsync(Guid articleId);
}