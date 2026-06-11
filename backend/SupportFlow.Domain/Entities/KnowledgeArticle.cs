using SupportFlow.Domain.Enums;

namespace SupportFlow.Domain.Entities;

public class KnowledgeArticle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public KnowledgeArticleCategory Category { get; set; } = KnowledgeArticleCategory.General;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}