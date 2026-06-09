using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Infrastructure.Persistence;
using SupportFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SupportFlow.Infrastructure.Tickets
{
    public class TicketService : ITicketService
    {

        private readonly AppDbContext _dbContext;

        public TicketService(AppDbContext context)
        {
            _dbContext = context;
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
            .FirstOrDefaultAsync(ticket => ticket.Id == id);

            if(ticket is null) return null;

            return ToDto(ticket);
        }

        public async Task<List<TicketDto>> GetTicketsAsync()
        {
            return await _dbContext.Tickets
            .OrderByDescending(ticket => ticket.CreatedAt)
            .Select(ticket => ToDto(ticket)).
            ToListAsync();
        }

        public async Task<TicketDto?> UpdateTicketAsync(Guid id, UpdateTicketDto request)
        {
            var ticket = await _dbContext.Tickets
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
                Sentiment = ticket.Sentiment,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            };
        }
    }
}