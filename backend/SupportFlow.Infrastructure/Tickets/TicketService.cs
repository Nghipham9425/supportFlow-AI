using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Infrastructure.Persistence;
using SupportFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SupportFlow.Domain.Enums;

namespace SupportFlow.Infrastructure.Tickets
{
    public class TicketService : ITicketService
    {

        private readonly AppDbContext _dbContext;
        private readonly ITicketActivityService _ticketActivityService;

        public TicketService(
            AppDbContext context,
            ITicketActivityService ticketActivityService)
        {
            _dbContext = context;
            _ticketActivityService = ticketActivityService;
        }


        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto request)
        {

            var ticket = new Ticket
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Subject = request.Subject,
                Description = request.Description,
                Channel = request.Channel
            };

            _dbContext.Tickets.Add(ticket);

            _ticketActivityService.Record(
                ticket.Id,
                TicketActivityType.Created,
                "Ticket created.");

            await _dbContext.SaveChangesAsync();

            return ToDto(ticket);
        }

        public async Task<bool> DeleteTicketAsync(Guid id)
        {
            var ticket = await _dbContext
                                .Tickets
                                .FirstOrDefaultAsync(ticket => ticket.Id == id);

            if (ticket is null) return false;
            _dbContext.Tickets.Remove(ticket);
            await _dbContext.SaveChangesAsync();
            return true;         
        }

        public async Task<TicketDto?> GetTicketByIdAsync(Guid id)
        {
            var ticket = await _dbContext.Tickets
            .Include(ticket => ticket.AssignedToUser)
            .FirstOrDefaultAsync(ticket => ticket.Id == id);

            if(ticket is null) return null;

            return ToDto(ticket);
        }

        public async Task<List<TicketDto>> GetTicketsAsync()
        {
            var tickets = await _dbContext.Tickets
                .Include(ticket => ticket.AssignedToUser)
                .OrderByDescending(ticket => ticket.CreatedAt)
                .ToListAsync();

            return tickets
                .Select(ToDto)
                .ToList();
        }

        public async Task<TicketDto?> UpdateTicketAsync(Guid id, UpdateTicketDto request)
        {
            var ticket = await _dbContext.Tickets
                        .Include(ticket => ticket.AssignedToUser)
                        .FirstOrDefaultAsync(ticket => ticket.Id == id);

            if (ticket is null) return null;
        
        if (request.CustomerName is not null)
        {
                ticket.CustomerName = request.CustomerName;
        }

        if (request.CustomerEmail is not null)
        {
            ticket.CustomerEmail = request.CustomerEmail;
        }

        if (request.Subject is not null)
        {
            ticket.Subject = request.Subject;
        }

        if (request.Description is not null)
        {
            ticket.Description = request.Description;
        }

        if (request.Channel is not null)
        {
            ticket.Channel = request.Channel.Value;
        }

        if (request.Category is not null)
        {
            ticket.Category = request.Category.Value;
        }

        if (request.Priority is not null)
        {
            ticket.Priority = request.Priority.Value;
        }

        if (request.Status is not null)
        {
            ticket.Status = request.Status.Value;
        }
        ticket.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return ToDto(ticket);
            
        }

        private static TicketDto ToDto(Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                CustomerName = ticket.CustomerName,
                CustomerEmail = ticket.CustomerEmail,
                Subject = ticket.Subject,
                Description = ticket.Description,
                Channel = ticket.Channel,
                Category = ticket.Category,
                Priority = ticket.Priority,
                Status = ticket.Status,
                AiSummary = ticket.AiSummary,
                AiDraftReply = ticket.AiDraftReply,
                Sentiment = ticket.Sentiment,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                AssignedToUserId = ticket.AssignedToUserId,
                AssignedToUserName = ticket.AssignedToUser?.Name,
                AssignedAt = ticket.AssignedAt,
            };
        }

        public async Task<TicketDto?> AssignToUserAsync(Guid TicketId, Guid UserId)
        {
            var ticket = await _dbContext.Tickets
            .Include(ticket => ticket.AssignedToUser)
            .FirstOrDefaultAsync(ticket => ticket.Id == TicketId);

            if (ticket is null) return null;

            var user = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == UserId &&
            user.Role == UserRole.Admin);

            if (user is null) return null;

            ticket.AssignedToUserId = user.Id;
            ticket.AssignedToUser = user;
            ticket.AssignedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            _ticketActivityService.Record(
                ticket.Id,
                TicketActivityType.Assigned,
                $"Ticket assigned to {user.Name}.",
                user.Id);

            await _dbContext.SaveChangesAsync();

            return ToDto(ticket);
        }
    }
}
