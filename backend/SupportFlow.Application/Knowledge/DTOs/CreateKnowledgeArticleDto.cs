using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Knowledge.DTOs;

public class CreateKnowledgeArticleDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public KnowledgeArticleCategory Category { get; set; } = KnowledgeArticleCategory.General;
}