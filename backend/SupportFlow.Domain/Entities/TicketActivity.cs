using SupportFlow.Domain.Enums;

namespace SupportFlow.Domain.Entities;

public class TicketActivity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public TicketActivityType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? ActorUserId { get; set; }
    public User? ActorUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}