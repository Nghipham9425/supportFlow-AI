using Microsoft.AspNetCore.Mvc;
using SupportFlow.Application.Dashboard.DTOs;
using SupportFlow.Application.Dashboard.Interfaces;

namespace SupportFlow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(
        CancellationToken cancellationToken = default)
    {
        var summary = await _dashboardService.GetSummaryAsync(cancellationToken);

        return Ok(summary);
    }
}
