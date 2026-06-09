using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Tickets.DTOs;
public class CreateTicketDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketChannel Channel { get; set; } = TicketChannel.Web;
}