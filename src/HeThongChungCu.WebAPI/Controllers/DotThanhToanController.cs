using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetDotThanhToanById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListDotThanhToan;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetLatestOpenDotThanhToan;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/dot-thanh-toan")]
public class DotThanhToanController : ApiControllerBase
{
    private readonly ISender _sender;

    public DotThanhToanController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách đợt thanh toán (hỗ trợ phân trang, lọc theo tháng, năm, trạng thái)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản trị viên xem danh sách các đợt thanh toán đã tạo.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DotThanhToanResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetListDotThanhToanQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết đợt thanh toán theo ID
    /// </summary>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<DotThanhToanDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetDotThanhToanByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy đợt thanh toán mới nhất đang mở (Nháp hoặc Đã phát hành)
    /// </summary>
    [HttpPost("get-latest-open")]
    [ProducesResponseType(typeof(ApiResponse<DotThanhToanDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLatestOpen([FromBody] GetLatestOpenDotThanhToanQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
