using HeThongChungCu.Application.Features.Dashboard.DTOs;

namespace HeThongChungCu.Application.Features.Dashboard.Queries.LayOverviewDashboard;

public record LayOverviewDashboardQuery(
    int? ToaNhaId = null,
    int? Thang = null,
    int? Nam = null,
    int? Ngay = null
) : IQuery<DashboardOverviewResponse>;
