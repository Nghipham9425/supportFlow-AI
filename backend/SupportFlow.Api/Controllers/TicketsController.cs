using Microsoft.AspNetCore.Mvc;
using SupportFlow.Application.Tickets;
using SupportFlow.Application.Tickets.DTOs;
using SupportFlow.Application.Tickets.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
    private readonly ITicketReplyService _ticketReplyService;
    private readonly ITicketActivityService _ticketActivityService;

    public TicketsController(
        ITicketService ticketService,
        ITicketAnalysisService ticketAnalysisService,
        ITicketDraftReplyService ticketDraftReplyService,
        IRelatedKnowledgeService relatedKnowledgeService,
        ITicketReplyService ticketReplyService,
        ITicketActivityService ticketActivityService
        )
    {
        _ticketService = ticketService;
        _ticketAnalysisService = ticketAnalysisService;
        _ticketDraftReplyService = ticketDraftReplyService;
        _relatedKnowledgeService = relatedKnowledgeService;
        _ticketReplyService = ticketReplyService;
        _ticketActivityService = ticketActivityService;
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

    [HttpPost("{id:guid}/assign-to-me")]
    public async Task<ActionResult<TicketDto>> AssignToMe(Guid id)
    {
        var userIdValue =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var ticket = await _ticketService.AssignToUserAsync(id, userId);

        if (ticket is null)
        {
            return NotFound();
        }

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
    public async Task<ActionResult<TicketDto>> AnalyzeTicket(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdValue =
                    User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var ticket = await _ticketAnalysisService.AnalyzeTicketAsync(id, userId, cancellationToken);

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
    public async Task<ActionResult<TicketDto>> GenerateDraftReply(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdValue =
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
                return Unauthorized();

            var ticket = await _ticketDraftReplyService.GenerateDraftReplyAsync(id, userId, cancellationToken);
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

    [HttpPost("{id:guid}/replies")]
    public async Task<ActionResult<TicketReplyDto>> SendReply(
        Guid id,
        SendTicketReplyDto request,
        CancellationToken cancellationToken)
    {
        var userIdValue =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var reply = await _ticketReplyService.SendReplyAsync(
                id,
                userId,
                request,
                cancellationToken);

            if (reply is null)
            {
                return NotFound();
            }

            return Ok(reply);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/replies")]
    public async Task<ActionResult<IReadOnlyList<TicketReplyDto>>> GetReplies(
        Guid id,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);

        if (ticket is null)
        {
            return NotFound();
        }

        var replies = await _ticketReplyService.GetRepliesAsync(
            id,
            cancellationToken);

        return Ok(replies);
    }

    [HttpGet("{id:guid}/activities")]
    public async Task<ActionResult<IReadOnlyList<TicketActivityDto>>> GetActivities(
        Guid id,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);

        if (ticket is null)
        {
            return NotFound();
        }

        var activities = await _ticketActivityService.GetActivitiesAsync(
            id,
            cancellationToken);

        return Ok(activities);
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<ActionResult<TicketActivityDto>> AddNote(
        Guid id,
        CreateTicketNoteDto request,
        CancellationToken cancellationToken)
    {
        var userIdValue =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var activity = await _ticketActivityService.AddNoteAsync(
                id,
                userId,
                request,
                cancellationToken);

            if (activity is null)
            {
                return NotFound();
            }

            return Ok(activity);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<TicketDto>> UpdateStatus(
        Guid id,
        UpdateTicketStatusDto request,
        CancellationToken cancellationToken = default
    )
    {
        var userIdValue =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }
        try
        {
            var ticket = await _ticketService.UpdateStatusAsync(id, request, userId, cancellationToken);
            if (ticket is null) return NotFound();
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
