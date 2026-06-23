using SupportFlow.Application.Knowledge.DTOs;

namespace SupportFlow.Application.Knowledge.Interfaces;

public interface IKnowledgeChunkService
{
    Task<List<KnowledgeChunkDto>> GetChunksByArticleIdAsync(Guid ArticleId);
    Task<List<KnowledgeChunkDto>?> RegenerateChunksAsync(Guid articleId);
}