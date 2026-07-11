using SupportFlow.Application.Tickets;
using SupportFlow.Application.Tickets.DTOs;

namespace SupportFlow.Application.AI.Interfaces;
public interface ITicketDraftReplyGenerator
{
        Task<string> GenerateDraftReplyAsync(TicketDto ticket, IReadOnlyList<RelatedKnowledgeDto> relatedKnowledge );
}