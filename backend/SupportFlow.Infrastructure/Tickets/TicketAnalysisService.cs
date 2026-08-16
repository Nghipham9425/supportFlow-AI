using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Domain.Entities;
using SupportFlow.Domain.Enums;
using SupportFlow.Infrastructure.Persistence;

namespace SupportFlow.Infrastructure.Tickets;

public class TicketAnalysisService : ITicketAnalysisService
{
    private readonly AppDbContext _dbContext;
    private readonly ITicketAnalyzer _ticketAnalyzer;
    private readonly ITicketActivityService _ticketActivityService;

    public TicketAnalysisService(AppDbContext dbContext, ITicketAnalyzer ticketAnalyzer, ITicketActivityService ticketActivityService)
    {
        _dbContext = dbContext;
        _ticketAnalyzer = ticketAnalyzer;
        _ticketActivityService = ticketActivityService;
    }

    public async Task<TicketDto?> AnalyzeTicketAsync(Guid ticketId,Guid actorUserId, CancellationToken cancellationToken=default)
    {
        var ticket = await _dbContext.Tickets
        .Include(ticket => ticket.AssignedToUser)
        .FirstOrDefaultAsync(ticket => ticket.Id == ticketId, cancellationToken);

        if (ticket is null)
        {
            return null;
        }

        var analysis = await _ticketAnalyzer.AnalyzeAsync(ToDto(ticket));

        ticket.AiSummary = analysis.Summary;
        ticket.Sentiment = analysis.Sentiment;
        ticket.Priority = analysis.SuggestedPriority;
        ticket.Category = analysis.SuggestedCategory;
        ticket.Status = TicketStatus.Analyzed;
        ticket.UpdatedAt = DateTime.UtcNow;

        _ticketActivityService.Record(
            ticketId,
            TicketActivityType.Analyzed,
            "AI analysis completed.",
            actorUserId
        );

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(ticket);
    }

    private static TicketDto ToDto(Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            CustomerName = ticket.CustomerName,
            CustomerEmail = ticket.CustomerEmail,
            Subject = ticket.Subject,
            Description = ticket.Description,
            Channel = ticket.Channel,
            Category = ticket.Category,
            Priority = ticket.Priority,
            Status = ticket.Status,
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUserName = ticket.AssignedToUser?.Name,
            AssignedAt = ticket.AssignedAt,
            AiSummary = ticket.AiSummary,
            AiDraftReply = ticket.AiDraftReply,
            Sentiment = ticket.Sentiment,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}
