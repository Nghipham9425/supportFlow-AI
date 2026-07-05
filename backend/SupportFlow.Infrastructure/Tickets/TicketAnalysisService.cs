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

    public TicketAnalysisService(AppDbContext dbContext, ITicketAnalyzer ticketAnalyzer)
    {
        _dbContext = dbContext;
        _ticketAnalyzer = ticketAnalyzer;
    }

    public async Task<TicketDto?> AnalyzeTicketAsync(Guid ticketId)
    {
        var ticket = await _dbContext.Tickets
            .FirstOrDefaultAsync(ticket => ticket.Id == ticketId);

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

        await _dbContext.SaveChangesAsync();

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
            Sentiment = ticket.Sentiment,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}
