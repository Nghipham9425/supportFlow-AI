using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Domain.Enums;

namespace SupportFlow.Infrastructure.AI;

public class FakeTicketAnalyzer : ITicketAnalyzer
{
    public Task<TicketAnalysisResultDto> AnalyzeAsync(TicketDto ticket)
    {
        var text = $"{ticket.Subject} {ticket.Description}".ToLowerInvariant();

        var category = InferCategory(text);
        var priority = InferPriority(text);
        var sentiment = InferSentiment(text);

        var summary = BuildSummary(ticket, category, priority, sentiment);

        return Task.FromResult(new TicketAnalysisResultDto
        {
            Summary = summary,
            Sentiment = sentiment,
            SuggestedPriority = priority,
            SuggestedCategory = category
        });
    }

    private static TicketCategory InferCategory(string text)
    {
        if (ContainsAny(text, "password", "login", "log in", "account", "email", "verify"))
        {
            return TicketCategory.AccountAccess;
        }

        if (ContainsAny(text, "refund", "invoice", "billing", "payment", "charge", "paid"))
        {
            return TicketCategory.Billing;
        }

        if (ContainsAny(text, "bug", "error", "crash", "broken", "not working"))
        {
            return TicketCategory.BugReport;
        }

        if (ContainsAny(text, "slow", "timeout", "technical", "server", "api"))
        {
            return TicketCategory.TechnicalIssue;
        }

        if (ContainsAny(text, "feature", "product", "how to", "question"))
        {
            return TicketCategory.ProductQuestion;
        }

        return TicketCategory.Other;
    }

    private static TicketPriority InferPriority(string text)
    {
        if (ContainsAny(text, "urgent", "asap", "critical", "production", "blocked", "cannot access"))
        {
            return TicketPriority.Critical;
        }

        if (ContainsAny(text, "cannot", "can't", "failed", "error", "angry", "refund"))
        {
            return TicketPriority.High;
        }

        if (ContainsAny(text, "question", "help", "guide", "how"))
        {
            return TicketPriority.Medium;
        }

        return TicketPriority.Low;
    }

    private static TicketSentiment InferSentiment(string text)
    {
        if (ContainsAny(text, "angry", "furious", "terrible", "hate", "unacceptable"))
        {
            return TicketSentiment.Angry;
        }

        if (ContainsAny(text, "frustrated", "annoyed", "upset", "again", "still"))
        {
            return TicketSentiment.Frustrated;
        }

        if (ContainsAny(text, "confused", "not sure", "don't understand", "unclear"))
        {
            return TicketSentiment.Confused;
        }

        return TicketSentiment.Neutral;
    }

    private static string BuildSummary(
        TicketDto ticket,
        TicketCategory category,
        TicketPriority priority,
        TicketSentiment sentiment)
    {
        var description = ticket.Description.Trim();
        var shortDescription = description.Length > 180
            ? $"{description[..180]}..."
            : description;

        return
            $"Mock AI analysis: The customer reports \"{ticket.Subject}\". " +
            $"Likely category is {category}, priority is {priority}, and sentiment is {sentiment}. " +
            $"Issue context: {shortDescription}";
    }

    private static bool ContainsAny(string text, params string[] keywords)
    {
        return keywords.Any(text.Contains);
    }
}
