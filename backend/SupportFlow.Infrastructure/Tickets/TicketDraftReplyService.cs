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

    public TicketDraftReplyService(
        AppDbContext db,
        ITicketDraftReplyGenerator draftReplyGenerator)
    {
        _db = db;
        _draftReplyGenerator = draftReplyGenerator;
    }
    public async Task<TicketDto?> GenerateDraftReplyAsync(Guid ticketId)
    {
        var ticket = await _db.Tickets.FirstOrDefaultAsync(ticket => ticket.Id == ticketId);
        if (ticket is null) return null;
        var draftReply = await _draftReplyGenerator.GenerateDraftReplyAsync(ToDto(ticket));

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
            AiSummary = ticket.AiSummary,
            AiDraftReply = ticket.AiDraftReply,
            Sentiment = ticket.Sentiment,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}