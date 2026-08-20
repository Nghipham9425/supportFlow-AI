using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Tickets.DTOs;
public class UpdateTicketDto
{
    public string? CustomerName{ get; set; }
    public string? CustomerEmail { get; set; }    
    public string? Subject { get; set; }
    public string? Description { get; set; }
    public TicketChannel? Channel { get; set; }
    public TicketCategory? Category { get; set; }
    public TicketPriority? Priority { get; set; }
}
