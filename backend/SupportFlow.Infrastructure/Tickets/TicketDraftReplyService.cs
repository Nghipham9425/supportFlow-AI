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

    public TicketDraftReplyService(
        AppDbContext db,
        ITicketDraftReplyGenerator draftReplyGenerator,
        IRelatedKnowledgeService relatedKnowledgeService)
    {
        _db = db;
        _draftReplyGenerator = draftReplyGenerator;
        _relatedKnowledgeService = relatedKnowledgeService;
    }
    public async Task<TicketDto?> GenerateDraftReplyAsync(Guid ticketId)
    {
        var ticket = await _db.Tickets
            .Include(ticket => ticket.AssignedToUser)
        .FirstOrDefaultAsync(ticket => ticket.Id == ticketId);
        if (ticket is null) return null;

        var ticketDto = ToDto(ticket);
        var relatedKnowledge =await _relatedKnowledgeService.GetForTicketAsync(ticketId);

        var draftReply =await _draftReplyGenerator.GenerateDraftReplyAsync(ticketDto, relatedKnowledge);     

        ticket.AiDraftReply = draftReply;
        ticket.Status = TicketStatus.Drafted;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

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
