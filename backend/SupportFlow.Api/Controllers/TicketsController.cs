using Microsoft.AspNetCore.Mvc;
using SupportFlow.Application.Tickets;
using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SupportFlow.Api.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize(Roles = "Admin")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ITicketAnalysisService _ticketAnalysisService;
    private readonly ITicketDraftReplyService _ticketDraftReplyService;
    private readonly IRelatedKnowledgeService _relatedKnowledgeService;

    public TicketsController(
        ITicketService ticketService,
        ITicketAnalysisService ticketAnalysisService,
        ITicketDraftReplyService ticketDraftReplyService,
        IRelatedKnowledgeService relatedKnowledgeService)
    {
        _ticketService = ticketService;
        _ticketAnalysisService = ticketAnalysisService;
        _ticketDraftReplyService = ticketDraftReplyService;
        _relatedKnowledgeService = relatedKnowledgeService;
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

    [AllowAnonymous]
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

    [HttpPost("{id:guid}/analyze")]
    public async Task<ActionResult<TicketDto>> AnalyzeTicket(Guid id)
    {
        try
        {
            var ticket = await _ticketAnalysisService.AnalyzeTicketAsync(id);

            if (ticket is null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {

            return Conflict(new { message = ex.Message });
        }

    }
    [HttpPost("{id:guid}/draft-reply")]
    public async Task<ActionResult<TicketDto>> GenerateDraftReply(Guid id)
    {
        try
        {
            var ticket = await _ticketDraftReplyService.GenerateDraftReplyAsync(id);
            if (ticket is null) return NotFound();
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {

            return Conflict(new { message = ex.Message });
        }

    }
    [HttpGet("{id:guid}/related-knowledge")]
    public async Task<ActionResult<IReadOnlyList<RelatedKnowledgeDto>>> GetRelatedKnowledge(Guid id,
    CancellationToken cancellationToken = default)
    {
        var relatedKnowledge = await _relatedKnowledgeService.GetForTicketAsync(id, cancellationToken);

        return Ok(relatedKnowledge);
    }
}
