using SupportFlow.Domain.Enums;

namespace SupportFlow.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketChannel Channel { get; set; } = TicketChannel.Web;

    public TicketCategory Category { get; set; } = TicketCategory.Other;

    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public string? AiSummary { get; set; }

    public string? AiDraftReply { get; set; }

    public TicketSentiment Sentiment { get; set; } = TicketSentiment.Unknown;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
