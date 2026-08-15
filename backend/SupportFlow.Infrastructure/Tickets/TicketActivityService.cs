using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Domain.Enums;
using SupportFlow.Infrastructure.Persistence;
using SupportFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SupportFlow.Infrastructure.Tickets;

public class TicketActivityService : ITicketActivityService
{
    private readonly AppDbContext _db;

    public TicketActivityService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TicketActivityDto?> AddNoteAsync(
        Guid ticketId,
        Guid actorUserId,
        CreateTicketNoteDto request,
        CancellationToken cancellationToken = default)
    {
        var content = request.Content?.Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException(
                "Internal note content is required.");
        }

        if (content.Length > 2000)
        {
            throw new InvalidOperationException(
                "Internal note cannot exceed 2000 characters.");
        }

        var ticketExists = await _db.Tickets.AnyAsync(
            ticket => ticket.Id == ticketId,
            cancellationToken);

        if (!ticketExists)
        {
            return null;
        }

        var actor = await _db.Users.FirstOrDefaultAsync(
            user => user.Id == actorUserId &&
                    user.Role == UserRole.Admin,
            cancellationToken);

        if (actor is null)
        {
            throw new UnauthorizedAccessException(
                "Only an admin can add an internal note.");
        }

        var activity = new TicketActivity
        {
            TicketId = ticketId,
            Type = TicketActivityType.InternalNote,
            Message = content,
            ActorUserId = actor.Id,
            CreatedAt = DateTime.UtcNow
        };

        _db.TicketActivities.Add(activity);
        await _db.SaveChangesAsync(cancellationToken);

        return ToDto(activity, actor.Name);
    }
    public async Task<IReadOnlyList<TicketActivityDto>> GetActivitiesAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await _db.TicketActivities
        .AsNoTracking()
        .Where(activity => activity.TicketId == ticketId)
        .OrderByDescending(activity => activity.CreatedAt)
        .Select(activity => new TicketActivityDto
        {
            Id = activity.Id,
            TicketId = activity.TicketId,
            Type = activity.Type,
            Message = activity.Message,
            ActorUserName = activity.ActorUser == null
                ? null
                : activity.ActorUser.Name,
            CreatedAt = activity.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public void Record(Guid ticketId, TicketActivityType type, string message, Guid? actorUserId = null, DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new InvalidOperationException("Activity message is required.");

        var content = message.Trim();
        if (content.Length > 2000)
            throw new InvalidOperationException(
                "Activity message cannot exceed 2000 characters.");

        _db.TicketActivities.Add(new TicketActivity
        {
            TicketId = ticketId,
            Type = type,
            Message = content,
            ActorUserId = actorUserId,
            CreatedAt = createdAt ?? DateTime.UtcNow
        });
    }

    private static TicketActivityDto ToDto(
        TicketActivity activity,
        string? actorUserName)
    {
        return new TicketActivityDto
        {
            Id = activity.Id,
            TicketId = activity.TicketId,
            Type = activity.Type,
            Message = activity.Message,
            ActorUserName = actorUserName,
            CreatedAt = activity.CreatedAt
        };
    }
}
