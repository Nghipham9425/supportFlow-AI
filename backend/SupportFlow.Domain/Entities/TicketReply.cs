namespace SupportFlow.Domain.Entities;

public class TicketReply
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TicketId { get; set; }

    public Ticket Ticket { get; set; } = null!;

    public Guid SentByUserId { get; set; }

    public User SentByUser { get; set; } = null!;

    public string RecipientEmail { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public string? ProviderMessageId { get; set; }
}
