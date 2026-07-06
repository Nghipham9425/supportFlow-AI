using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Tickets.DTOs;

namespace SupportFlow.Infrastructure.AI;

public class FakeTicketDraftReplyGenerator : ITicketDraftReplyGenerator
{
    public Task<string> GenerateDraftReplyAsync(TicketDto ticket)
    {
        var summary = string.IsNullOrWhiteSpace(ticket.AiSummary)
        ? "I reviewed your request and understand the issue you are experiencing."
        : ticket.AiSummary;

        var draft =
            $"Hi {ticket.CustomerName},\n\n" +
            "Thanks for reaching out to SupportFlow.\n\n" +
            $"{summary}\n\n" +
            "I recommend the following next step: please confirm any recent changes related to this issue, " +
            "and our support team will continue investigating based on the details you provided.\n\n" +
            "Best regards,\n" +
            "SupportFlow Support Team";

        return Task.FromResult(draft);
    }
}