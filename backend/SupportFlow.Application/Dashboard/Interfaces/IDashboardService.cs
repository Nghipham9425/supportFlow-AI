using SupportFlow.Application.Dashboard.DTOs;

namespace SupportFlow.Application.Dashboard.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
