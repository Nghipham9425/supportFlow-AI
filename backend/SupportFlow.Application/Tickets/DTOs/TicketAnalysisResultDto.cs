using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Tickets.DTOs;

public class TicketAnalysisResultDto
{
    public string Summary { get; set; } = string.Empty;
    public TicketSentiment Sentiment { get; set; }
    public TicketPriority SuggestedPriority { get; set; }
    public TicketCategory SuggestedCategory { get; set; }
}
