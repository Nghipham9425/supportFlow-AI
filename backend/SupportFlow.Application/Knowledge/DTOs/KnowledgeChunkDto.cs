namespace SupportFlow.Application.Knowledge.DTOs;

public class KnowledgeChunkDto
{
    public Guid Id { get; set; }

    public Guid KnowledgeArticleId { get; set; }

    public string Content { get; set; } = string.Empty;

    public int ChunkIndex { get; set; }

    public bool IsEmbedded { get; set; }

    public DateTime? EmbeddedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
