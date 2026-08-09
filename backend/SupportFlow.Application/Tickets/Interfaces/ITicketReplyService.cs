using SupportFlow.Application.Tickets.DTOs;

namespace SupportFlow.Application.Tickets.Interfaces;

public interface ITicketReplyService
{
    Task<TicketReplyDto?> SendReplyAsync(
        Guid ticketId,
        Guid sentByUserId,
        SendTicketReplyDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TicketReplyDto>> GetRepliesAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default);
}
