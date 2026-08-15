using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Tickets.DTOs;

public class TicketActivityDto
{
    public Guid Id { get; set; }

    public Guid TicketId { get; set; }

    public TicketActivityType Type { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? ActorUserName { get; set; }

    public DateTime CreatedAt { get; set; }
}