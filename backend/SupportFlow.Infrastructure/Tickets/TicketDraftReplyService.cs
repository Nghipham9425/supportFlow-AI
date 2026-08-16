using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Domain.Entities;
using SupportFlow.Domain.Enums;
using SupportFlow.Infrastructure.Persistence;

namespace SupportFlow.Infrastructure.Tickets;

public class TicketDraftReplyService : ITicketDraftReplyService
{
    private readonly AppDbContext _db;
    private readonly ITicketDraftReplyGenerator _draftReplyGenerator;
    private readonly IRelatedKnowledgeService _relatedKnowledgeService;

    private readonly ITicketActivityService _ticketActivityService;

    public TicketDraftReplyService(
        AppDbContext db,
        ITicketDraftReplyGenerator draftReplyGenerator,
        IRelatedKnowledgeService relatedKnowledgeService,
        ITicketActivityService ticketActivityService)
    {
        _db = db;
        _draftReplyGenerator = draftReplyGenerator;
        _relatedKnowledgeService = relatedKnowledgeService;
        _ticketActivityService = ticketActivityService;
    }
    public async Task<TicketDto?> GenerateDraftReplyAsync(Guid ticketId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets
            .Include(ticket => ticket.AssignedToUser)
        .FirstOrDefaultAsync(ticket => ticket.Id == ticketId, cancellationToken);
        if (ticket is null) return null;

        var ticketDto = ToDto(ticket);
        var relatedKnowledge =await _relatedKnowledgeService.GetForTicketAsync(ticketId, cancellationToken);

        var draftReply =await _draftReplyGenerator.GenerateDraftReplyAsync(ticketDto, relatedKnowledge);     

        ticket.AiDraftReply = draftReply;
        ticket.Status = TicketStatus.Drafted;
        ticket.UpdatedAt = DateTime.UtcNow;

        _ticketActivityService.Record(ticketId, TicketActivityType.DraftGenerated, "AI draft reply generated.", actorUserId);

        await _db.SaveChangesAsync(cancellationToken);

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
