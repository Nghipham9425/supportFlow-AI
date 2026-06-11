using SupportFlow.Application.Knowledge.DTOs;

namespace SupportFlow.Application.Knowledge.Interfaces;

public interface IKnowledgeArticleService
{
    Task<List<KnowledgeArticleDto>> GetArticlesAsync();
    Task<KnowledgeArticleDto?> GetArticleByIdAsync(Guid id);
    Task<KnowledgeArticleDto> CreateArticleAsync(CreateKnowledgeArticleDto request);
    Task<KnowledgeArticleDto?> UpdateArticleAsync(Guid id, UpdateKnowledgeArticleDto request);
    Task<bool> DeleteArticleAsync(Guid id);
}
