namespace SupportFlow.Application.Dashboard.DTOs;

public class DashboardSummaryDto
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int DraftedTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int HighPriorityTickets { get; set; }

    public int TotalKnowledgeArticles { get; set; }
    public int AiReadyKnowledgeArticles { get; set; }
    public int TotalKnowledgeChunks { get; set; }
    public int EmbeddedKnowledgeChunks { get; set; }
}
