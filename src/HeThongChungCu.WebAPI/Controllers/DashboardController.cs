using Asp.Versioning;
using HeThongChungCu.Application.Features.Dashboard.DTOs;
using HeThongChungCu.Application.Features.Dashboard.Queries.LayOverviewDashboard;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiVersion("1.0")]
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ApiControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy thông tin tổng hợp cho Dashboard (Overview)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi người quản trị đăng nhập vào trang chủ/dashboard và muốn xem báo cáo tổng hợp.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Không bắt buộc**: `ToaNhaId` (lọc theo tòa nhà), `Thang` (lọc theo tháng), `Nam` (lọc theo năm).
    /// </remarks>
    [HttpPost("overview")]
    [ProducesResponseType(typeof(ApiResponse<DashboardOverviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOverview([FromBody] LayOverviewDashboardQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
