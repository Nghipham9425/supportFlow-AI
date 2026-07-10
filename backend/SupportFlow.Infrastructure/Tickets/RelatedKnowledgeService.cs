using System.Xml;
using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.Tickets;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Domain.Entities;
using SupportFlow.Infrastructure.Persistence;

public class RelatedKnowledgeService : IRelatedKnowledgeService
{
    private readonly AppDbContext _db;
    public RelatedKnowledgeService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<IReadOnlyList<RelatedKnowledgeDto>> GetForTicketAsync(Guid TicketId, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets.FirstOrDefaultAsync(ticket => ticket.Id == TicketId, cancellationToken);
        if (ticket is null) return [];
        var ticketText = $"{ticket.Subject} {ticket.Description} {ticket.AiSummary}";

        var chunks = await _db.KnowledgeChunks
        .Include(chunk => chunk.KnowledgeArticle)
        .ToListAsync(cancellationToken);
        // bước tiếp theo: tokenize + score
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
    private static double CalculateScore(HashSet<string> ticketWords, string chunkContent)
    {
        var chunkWords = Tokenize(chunkContent);
        if (chunkWords.Count == 0) return 0;
        return chunkWords.Count(ticketWords.Contains);
    }
}