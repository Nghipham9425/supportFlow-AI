using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Tickets;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Infrastructure.Persistence;

public class RelatedKnowledgeService : IRelatedKnowledgeService
{
    private readonly AppDbContext _db;
    private readonly IEmbeddingProvider _embeddingProvider;

    public RelatedKnowledgeService(
        AppDbContext db,
        IEmbeddingProvider embeddingProvider)
    {
        _db = db;
        _embeddingProvider = embeddingProvider;
    }

    public async Task<IReadOnlyList<RelatedKnowledgeDto>> GetForTicketAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets
            .FirstOrDefaultAsync(ticket => ticket.Id == ticketId, cancellationToken);

        if (ticket is null)
        {
            return [];
        }

        var ticketText = $"{ticket.Subject} {ticket.Description} {ticket.AiSummary}";

        try
        {
            var vectorMatches = await GetVectorMatchesAsync(
                ticketText,
                cancellationToken);

            if (vectorMatches.Count > 0)
            {
                return vectorMatches;
            }
        }
        catch (InvalidOperationException)
        {
            // Fall back to keyword search when embeddings are unavailable.
        }

        return await GetKeywordMatchesAsync(ticketText, cancellationToken);
    }

    private async Task<IReadOnlyList<RelatedKnowledgeDto>> GetKeywordMatchesAsync(
        string ticketText,
        CancellationToken cancellationToken)
    {
        var chunks = await _db.KnowledgeChunks
            .Include(chunk => chunk.KnowledgeArticle)
            .ToListAsync(cancellationToken);

        var ticketWords = Tokenize(ticketText);

        return chunks
            .Select(chunk => new RelatedKnowledgeDto(
                chunk.KnowledgeArticleId,
                chunk.KnowledgeArticle.Title,
                chunk.Id,
                chunk.Content,
                CalculateScore(ticketWords, chunk.Content)))
            .Where(result => result.Score > 0)
            .OrderByDescending(result => result.Score)
            .Take(5)
            .ToList();
    }

    private async Task<IReadOnlyList<RelatedKnowledgeDto>> GetVectorMatchesAsync(
        string ticketText,
        CancellationToken cancellationToken)
    {
        var ticketEmbedding = await _embeddingProvider.GenerateEmbeddingAsync(ticketText);
        var ticketVector = new Vector(ticketEmbedding);

        var chunks = await _db.KnowledgeChunks
            .Include(chunk => chunk.KnowledgeArticle)
            .Where(chunk => chunk.Embedding != null)
            .OrderBy(chunk => chunk.Embedding!.L2Distance(ticketVector))
            .Take(5)
            .Select(chunk => new
            {
                chunk.KnowledgeArticleId,
                ArticleTitle = chunk.KnowledgeArticle.Title,
                ChunkId = chunk.Id,
                chunk.Content,
                Distance = chunk.Embedding!.L2Distance(ticketVector)
            })
            .ToListAsync(cancellationToken);

        return chunks
            .Select(result => new RelatedKnowledgeDto(
                result.KnowledgeArticleId,
                result.ArticleTitle,
                result.ChunkId,
                result.Content,
                1.0 / (1.0 + result.Distance)))
            .ToList();
    }

    private static HashSet<string> Tokenize(string text)
    {
        return text
            .ToLowerInvariant()
            .Split(
                [' ', '.', ',', ';', ':', '!', '?', '\r', '\n', '\t', '-', '_', '/', '\\', '(', ')'],
                StringSplitOptions.RemoveEmptyEntries)
            .Where(word => word.Length >= 3)
            .ToHashSet();
    }

    private static double CalculateScore(
        HashSet<string> ticketWords,
        string chunkContent)
    {
        var chunkWords = Tokenize(chunkContent);
        if (chunkWords.Count == 0) return 0;
        return chunkWords.Count(ticketWords.Contains);
    }
}
