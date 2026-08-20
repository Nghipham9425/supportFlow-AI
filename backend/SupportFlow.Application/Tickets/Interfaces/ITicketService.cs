using SupportFlow.Application.Tickets.DTOs;
namespace SupportFlow.Application.Tickets.Interfaces;

public interface ITicketService
{
    Task<List<TicketDto>> GetTicketsAsync();
    Task<TicketDto?> GetTicketByIdAsync(Guid id);
    Task<TicketDto> CreateTicketAsync(CreateTicketDto request);
    Task<TicketDto?> UpdateTicketAsync(Guid id, UpdateTicketDto request);
    Task<TicketDto?> UpdateStatusAsync(
        Guid ticketId,
        UpdateTicketStatusDto request,
        Guid actorUserId,
        CancellationToken cancellationToken = default);
    Task<TicketDto?> AssignToUserAsync(Guid ticketId, Guid userId);
    Task<bool> DeleteTicketAsync(Guid id);
}
