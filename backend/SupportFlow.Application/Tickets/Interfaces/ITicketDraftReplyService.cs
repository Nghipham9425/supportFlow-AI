using SupportFlow.Application.Tickets.DTOs;

namespace SupportFlow.Application.Tickets.Interfaces;

public interface ITicketDraftReplyService
{
    Task<TicketDto?> GenerateDraftReplyAsync(Guid ticketId, Guid actorUserId, CancellationToken cancellationToken = default);
}