namespace SupportFlow.Domain.Entities;

public class KnowledgeChunk
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid KnowledgeArticleId { get; set; }

    public KnowledgeArticle KnowledgeArticle { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public int ChunkIndex { get; set; }

    public bool IsEmbedded { get; set; } = false;

    public DateTime? EmbeddedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
