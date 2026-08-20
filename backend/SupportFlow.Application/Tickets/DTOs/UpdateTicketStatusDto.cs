using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Tickets.DTOs;

public class UpdateTicketStatusDto
{
    public TicketStatus Status { get; set; }
}