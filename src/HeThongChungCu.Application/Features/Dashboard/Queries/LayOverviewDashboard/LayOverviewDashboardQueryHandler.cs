using HeThongChungCu.Application.Features.Dashboard.DTOs;

namespace HeThongChungCu.Application.Features.Dashboard.Queries.LayOverviewDashboard;

public class LayOverviewDashboardQueryHandler : IQueryHandler<LayOverviewDashboardQuery, DashboardOverviewResponse>
{
    private readonly IDashboardQueryRepository _dashboardRepository;

    public LayOverviewDashboardQueryHandler(IDashboardQueryRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<Result<DashboardOverviewResponse>> Handle(LayOverviewDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentThang = request.Thang ?? DateTimeOffset.UtcNow.Month;
        var currentNam = request.Nam ?? DateTimeOffset.UtcNow.Year;
        var currentNgay = request.Ngay;

        var cleanRequest = request with { Thang = currentThang, Nam = currentNam, Ngay = currentNgay };

        var overview = await _dashboardRepository.GetOverviewAsync(cleanRequest, cancellationToken);
        return Result<DashboardOverviewResponse>.Success(overview);
    }
}
