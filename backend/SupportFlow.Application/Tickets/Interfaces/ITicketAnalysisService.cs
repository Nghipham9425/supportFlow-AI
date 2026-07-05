using SupportFlow.Application.Tickets.DTOs;

namespace SupportFlow.Application.Tickets.Interfaces;

public interface ITicketAnalysisService
{
    Task<TicketDto?> AnalyzeTicketAsync(Guid ticketId);
}
