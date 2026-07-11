using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Tickets;
using SupportFlow.Application.Tickets.DTOs;

namespace SupportFlow.Infrastructure.AI;

public class FakeTicketDraftReplyGenerator : ITicketDraftReplyGenerator
{
    public Task<string> GenerateDraftReplyAsync(TicketDto ticket,IReadOnlyList<RelatedKnowledgeDto> relatedKnowledge)
    {
        var summary = string.IsNullOrWhiteSpace(ticket.AiSummary)
        ? "I reviewed your request and understand the issue you are experiencing."
        : ticket.AiSummary;

        var knowledgeContext = relatedKnowledge.Count == 0
        ? "I could not find a matching knowledge base article yet, so I will continue based on the ticket details."
        : "I found the following relevant support guidance: " +
        string.Join(" ", relatedKnowledge
                            .Take(2)
                            .Select(item => $"{item.ArticleTitle} : {item.Content}"));
        var draft =
            $"Hi {ticket.CustomerName},\n\n" +
            "Thanks for reaching out to SupportFlow.\n\n" +
            $"{summary}\n\n" +
            $"{knowledgeContext}\n\n" +
            "I recommend the following next step: please confirm any recent changes related to this issue, " +
            "and our support team will continue investigating based on the details you provided.\n\n" +
            "Best regards,\n" +
            "SupportFlow Support Team";

        return Task.FromResult(draft);
    }
}