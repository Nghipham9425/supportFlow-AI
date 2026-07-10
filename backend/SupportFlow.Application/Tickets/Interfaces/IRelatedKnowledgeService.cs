using SupportFlow.Application.Tickets;

namespace SupportFlow.Application.Tickets.Interfaces;

public interface IRelatedKnowledgeService
{
    Task<IReadOnlyList<RelatedKnowledgeDto>> GetForTicketAsync(Guid TicketId, CancellationToken cancellationToken = default);
}