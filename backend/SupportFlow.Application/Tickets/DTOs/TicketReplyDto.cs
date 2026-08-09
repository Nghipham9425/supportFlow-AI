namespace SupportFlow.Application.Tickets.DTOs;

public class TicketReplyDto
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string SentByUserName { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
