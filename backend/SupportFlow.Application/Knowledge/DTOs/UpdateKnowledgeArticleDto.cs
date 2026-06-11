using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Knowledge.DTOs;

public class UpdateKnowledgeArticleDto
{
    public string? Title { get; set; } 
    public string? Content { get; set; }
    public KnowledgeArticleCategory? Category { get; set; }
}