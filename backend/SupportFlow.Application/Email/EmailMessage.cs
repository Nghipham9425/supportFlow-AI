namespace SupportFlow.Application.Email;

public record EmailMessage(
    string RecipientEmail,
    string Subject,
    string HtmlContent);