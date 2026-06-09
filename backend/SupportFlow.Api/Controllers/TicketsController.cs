using Microsoft.AspNetCore.Mvc;
using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;

namespace SupportFlow.Api.Controllers;

[ApiController]
[Route("api/tickets")]

public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }
    [HttpGet]
    public async Task<ActionResult<List<TicketDto>>> GetTickets()
    {
        var tickets = await _ticketService.GetTicketsAsync();

        return Ok(tickets);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null) return NotFound();

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> CreateTicket(CreateTicketDto request)
    {
        var ticket = await _ticketService.CreateTicketAsync(request);

        return CreatedAtAction(
            nameof(GetTicket),
            new { id = ticket.Id },
            ticket);
    }

    
     [HttpPatch("{id:guid}")]
    public async Task<ActionResult<TicketDto>> UpdateTicket(
        Guid id,
        UpdateTicketDto request)
    {
        var ticket = await _ticketService.UpdateTicketAsync(id, request);

        if (ticket is null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        var deleted = await _ticketService.DeleteTicketAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

}