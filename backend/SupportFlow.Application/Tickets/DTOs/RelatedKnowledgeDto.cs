namespace SupportFlow.Application.Tickets;

public sealed record RelatedKnowledgeDto(
    Guid ArticleId,
    string ArticleTitle,
    Guid ChunkId,
    string Content,
    double Score
);