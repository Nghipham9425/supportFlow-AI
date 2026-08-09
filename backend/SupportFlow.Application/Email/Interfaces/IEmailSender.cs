namespace SupportFlow.Application.Email.Interfaces;

public interface IEmailSender
{
    Task<string?> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);
}