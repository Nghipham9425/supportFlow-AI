
using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Tickets.DTOs;

public class TicketDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketChannel Channel { get; set; }
    public TicketCategory Category { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public string? AiSummary { get; set; }
    public string? AiDraftReply { get; set; }
    public TicketSentiment Sentiment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}