using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.Dashboard.DTOs;
using SupportFlow.Application.Dashboard.Interfaces;
using SupportFlow.Domain.Enums;
using SupportFlow.Infrastructure.Persistence;

namespace SupportFlow.Infrastructure.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    public DashboardService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalTickets = await _db.Tickets.CountAsync(cancellationToken);
        var openTickets = await _db.Tickets.CountAsync(ticket => ticket.Status == TicketStatus.Open, cancellationToken);
        var draftedTickets = await _db.Tickets.CountAsync(ticket => ticket.Status == TicketStatus.Drafted, cancellationToken);
        var resolvedTickets = await _db.Tickets.CountAsync(ticket => ticket.Status == TicketStatus.Resolved, cancellationToken);
        var highPriorityTickets = await _db.Tickets
            .CountAsync(ticket => ticket.Priority >= TicketPriority.High, cancellationToken);

        var totalKnowledgeArticles = await _db.KnowledgeArticles.CountAsync(cancellationToken);
        var totalKnowledgeChunks = await _db.KnowledgeChunks.CountAsync(cancellationToken);
        var embeddedKnowledgeChunks = await _db.KnowledgeChunks
            .CountAsync(chunk => chunk.IsEmbedded, cancellationToken);

        var aiReadyKnowledgeArticleIds = await _db.KnowledgeChunks
            .Where(chunk => chunk.IsEmbedded)
            .Select(chunk => chunk.KnowledgeArticleId)
            .Distinct()
            .CountAsync(cancellationToken);

        return new DashboardSummaryDto
        {
            TotalTickets = totalTickets,
            OpenTickets = openTickets,
            DraftedTickets = draftedTickets,
            ResolvedTickets = resolvedTickets,
            HighPriorityTickets = highPriorityTickets,
            TotalKnowledgeArticles = totalKnowledgeArticles,
            AiReadyKnowledgeArticles = aiReadyKnowledgeArticleIds,
            TotalKnowledgeChunks = totalKnowledgeChunks,
            EmbeddedKnowledgeChunks = embeddedKnowledgeChunks
        };
    }
}
