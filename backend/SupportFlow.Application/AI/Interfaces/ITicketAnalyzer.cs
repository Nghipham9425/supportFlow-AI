using SupportFlow.Application.Tickets.DTOs;

namespace SupportFlow.Application.AI.Interfaces;

public interface ITicketAnalyzer
{
    Task<TicketAnalysisResultDto> AnalyzeAsync(TicketDto ticket);
}
