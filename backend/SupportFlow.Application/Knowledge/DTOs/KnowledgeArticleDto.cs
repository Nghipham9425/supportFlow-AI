using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Knowledge.DTOs;

public class KnowledgeArticleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public KnowledgeArticleCategory Category { get; set; }
    public int ChunkCount { get; set; }
    public bool IsAiReady { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}