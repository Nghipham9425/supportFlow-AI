using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Knowledge.DTOs;
using SupportFlow.Application.Knowledge.Interfaces;
using SupportFlow.Domain.Entities;
using SupportFlow.Infrastructure.Persistence;
using Pgvector;

namespace SupportFlow.Infrastructure.Knowledge;

public class KnowledgeEmbeddingService : IKnowledgeEmbeddingService
{
    private readonly AppDbContext _dbContext;
    private readonly IEmbeddingProvider _embeddingProvider;

    public KnowledgeEmbeddingService(
        AppDbContext dbContext,
        IEmbeddingProvider embeddingProvider)
    {
        _dbContext = dbContext;
        _embeddingProvider = embeddingProvider;
    }

    public async Task<List<KnowledgeChunkDto>?> GenerateEmbeddingsAsync(Guid articleId)
    {
        var articleExists = await _dbContext.KnowledgeArticles
            .AnyAsync(article => article.Id == articleId);

        if (!articleExists)
        {
            return null;
        }

        var chunks = await _dbContext.KnowledgeChunks
            .Where(chunk => chunk.KnowledgeArticleId == articleId)
            .OrderBy(chunk => chunk.ChunkIndex)
            .ToListAsync();

        if (chunks.Count == 0)
        {
            throw new InvalidOperationException(
                "Prepare article chunks before generating embeddings.");
        }

        foreach (var chunk in chunks)
        {
            var embedding = await _embeddingProvider.GenerateEmbeddingAsync(chunk.Content);

            chunk.Embedding = new Vector(embedding);
            chunk.IsEmbedded = true;
            chunk.EmbeddedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        return chunks.Select(ToDto).ToList();
    }

    private static KnowledgeChunkDto ToDto(KnowledgeChunk chunk)
    {
        return new KnowledgeChunkDto
        {
            Id = chunk.Id,
            KnowledgeArticleId = chunk.KnowledgeArticleId,
            Content = chunk.Content,
            ChunkIndex = chunk.ChunkIndex,
            IsEmbedded = chunk.IsEmbedded,
            EmbeddedAt = chunk.EmbeddedAt,
            CreatedAt = chunk.CreatedAt
        };
    }
}
