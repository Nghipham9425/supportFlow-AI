using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.Knowledge.DTOs;
using SupportFlow.Application.Knowledge.Interfaces;
using SupportFlow.Domain.Entities;
using SupportFlow.Infrastructure.Persistence;

namespace SupportFlow.Infrastructure.Knowledge;

public class KnowledgeChunkService : IKnowledgeChunkService
{
    private const int MaxChunkLength = 800;

    private readonly AppDbContext _dbContext;

    public KnowledgeChunkService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<KnowledgeChunkDto>> GetChunksByArticleIdAsync(Guid articleId)
    {
        return await _dbContext.KnowledgeChunks
            .Where(chunk => chunk.KnowledgeArticleId == articleId)
            .OrderBy(chunk => chunk.ChunkIndex)
            .Select(chunk => ToDto(chunk))
            .ToListAsync();
    }

    public async Task<List<KnowledgeChunkDto>?> RegenerateChunksAsync(Guid articleId)
    {
        var article = await _dbContext.KnowledgeArticles
            .FirstOrDefaultAsync(article => article.Id == articleId);

        if (article is null)
        {
            return null;
        }

        var existingChunks = await _dbContext.KnowledgeChunks
            .Where(chunk => chunk.KnowledgeArticleId == articleId)
            .ToListAsync();

        _dbContext.KnowledgeChunks.RemoveRange(existingChunks);

        var chunkContents = SplitIntoChunks(article.Content);

        var chunks = chunkContents
            .Select((content, index) => new KnowledgeChunk
            {
                KnowledgeArticleId = article.Id,
                Content = content,
                ChunkIndex = index
            })
            .ToList();

        _dbContext.KnowledgeChunks.AddRange(chunks);

        await _dbContext.SaveChangesAsync();

        return chunks
            .OrderBy(chunk => chunk.ChunkIndex)
            .Select(chunk => ToDto(chunk))
            .ToList();
    }

    private static List<string> SplitIntoChunks(string content)
    {
        var normalizedContent = content.Trim();

        if (string.IsNullOrWhiteSpace(normalizedContent))
        {
            return new List<string>();
        }
        var paragraphs = normalizedContent
        .Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
        .Select(paragraphs => paragraphs.Trim())
        .Where(paragraphs => !string.IsNullOrWhiteSpace(paragraphs))
        .ToList();

        var chunks = new List<string>();
        var currentChunk = string.Empty;

        foreach (var paragraph in paragraphs)
        {
            if (paragraph.Length > MaxChunkLength)
            {
                AddCurrentChunkIfNotEmpty(chunks, ref currentChunk);

                var splitLongParagraph = SplitLongText(paragraph);

                chunks.AddRange(splitLongParagraph);
                continue;
            }
            var candidate = string.IsNullOrWhiteSpace(currentChunk)
            ? paragraph
            : $"{currentChunk}\n\n{paragraph}";

            if (candidate.Length <= MaxChunkLength)
            {
                currentChunk = candidate;
            }
            else
            {
                AddCurrentChunkIfNotEmpty(chunks, ref currentChunk);
                currentChunk = paragraph;
            }
        }

        AddCurrentChunkIfNotEmpty(chunks, ref currentChunk);

        return chunks;
    }

    private static void AddCurrentChunkIfNotEmpty(
    List<string> chunks,
    ref string currentChunk)
    {
        if (!string.IsNullOrWhiteSpace(currentChunk))
        {
            chunks.Add(currentChunk.Trim());
            currentChunk = string.Empty;
        }
    }

    private static List<string> SplitLongText(string text)
    {
        var chunks = new List<string>();

        for (var start = 0; start < text.Length; start += MaxChunkLength)
        {
            var length = Math.Min(MaxChunkLength, text.Length - start);
            var chunk = text.Substring(start, length).Trim();

            if (!string.IsNullOrWhiteSpace(chunk))
            {
                chunks.Add(chunk);
            }
        }

        return chunks;
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
