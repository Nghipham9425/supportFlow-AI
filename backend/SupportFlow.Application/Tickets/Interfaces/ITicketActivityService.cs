using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Tickets.Interfaces;

public interface ITicketActivityService
{
    void Record(
        Guid ticketId,
        TicketActivityType type,
        string message,
        Guid? actorUserId = null,
        DateTime? createdAt = null);

    Task<IReadOnlyList<TicketActivityDto>> GetActivitiesAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default);

    Task<TicketActivityDto?> AddNoteAsync(
        Guid ticketId,
        Guid actorUserId,
        CreateTicketNoteDto request,
        CancellationToken cancellationToken = default);
}