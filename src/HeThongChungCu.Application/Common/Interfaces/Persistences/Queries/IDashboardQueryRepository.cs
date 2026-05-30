using HeThongChungCu.Application.Features.Dashboard.DTOs;
using HeThongChungCu.Application.Features.Dashboard.Queries.LayOverviewDashboard;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IDashboardQueryRepository
{
    Task<DashboardOverviewResponse> GetOverviewAsync(LayOverviewDashboardQuery query, CancellationToken cancellationToken = default);
}
