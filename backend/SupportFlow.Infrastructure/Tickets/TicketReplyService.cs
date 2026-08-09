using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.Email;
using SupportFlow.Application.Email.Interfaces;
using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Domain.Entities;
using SupportFlow.Domain.Enums;
using SupportFlow.Infrastructure.Persistence;

namespace SupportFlow.Infrastructure.Tickets;

public class TicketReplyService : ITicketReplyService
{
    private readonly AppDbContext _db;
    private readonly IEmailSender _emailSender;

    public TicketReplyService(AppDbContext db,IEmailSender emailSender)
    {
        _db = db;
        _emailSender = emailSender;
    }

   public async Task<TicketReplyDto?> SendReplyAsync(
        Guid ticketId,
        Guid sentByUserId,
        SendTicketReplyDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new InvalidOperationException("Reply content is required.");
        }

        var ticket = await _db.Tickets
            .FirstOrDefaultAsync(
                ticket => ticket.Id == ticketId,
                cancellationToken);

        if (ticket is null)
        {
            return null;
        }

        var sender = await _db.Users
            .FirstOrDefaultAsync(
                user => user.Id == sentByUserId &&
                        user.Role == UserRole.Admin,
                cancellationToken);

        if (sender is null)
        {
            throw new UnauthorizedAccessException(
                "Only an admin can send a ticket reply.");
        }

        var content = request.Content.Trim();
        var subject = $"Re: {ticket.Subject}";

        var messageId = await _emailSender.SendAsync(
            new EmailMessage(
                ticket.CustomerEmail,
                subject,
                BuildHtmlContent(content)),
            cancellationToken);

        var reply = new TicketReply
        {
            TicketId = ticket.Id,
            SentByUserId = sender.Id,
            RecipientEmail = ticket.CustomerEmail,
            Subject = subject,
            Content = content,
            SentAt = DateTime.UtcNow,
            ProviderMessageId = messageId
        };

        ticket.Status = TicketStatus.PendingCustomer;
        ticket.UpdatedAt = DateTime.UtcNow;

        _db.TicketReplies.Add(reply);
        await _db.SaveChangesAsync(cancellationToken);

        return ToDto(reply, sender.Name);
    }
    public async Task<IReadOnlyList<TicketReplyDto>> GetRepliesAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        return await _db.TicketReplies
            .Where(reply => reply.TicketId == ticketId)
            .Include(reply => reply.SentByUser)
            .OrderByDescending(reply => reply.SentAt)
            .Select(reply => new TicketReplyDto
            {
                Id = reply.Id,
                TicketId = reply.TicketId,
                SentByUserName = reply.SentByUser.Name,
                RecipientEmail = reply.RecipientEmail,
                Subject = reply.Subject,
                Content = reply.Content,
                SentAt = reply.SentAt
            })
            .ToListAsync(cancellationToken);
    }

    private static string BuildHtmlContent(string content)
    {
        var safeContent = HtmlEncoder.Default
            .Encode(content)
            .ReplaceLineEndings("<br />");

        return $"<p>{safeContent}</p>";
    }

    private static TicketReplyDto ToDto(
        TicketReply reply,
        string senderName)
    {
        return new TicketReplyDto
        {
            Id = reply.Id,
            TicketId = reply.TicketId,
            SentByUserName = senderName,
            RecipientEmail = reply.RecipientEmail,
            Subject = reply.Subject,
            Content = reply.Content,
            SentAt = reply.SentAt
        };
    }
}