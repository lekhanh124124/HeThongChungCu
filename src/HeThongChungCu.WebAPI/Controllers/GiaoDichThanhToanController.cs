using Asp.Versioning;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.GhiNhanGiaoDichThanhToan;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.TaoPhienThanhToanOnline;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.XacNhanThanhToanOnline;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetGiaoDichThanhToanByHoaDonId;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/giao-dich-thanh-toan")]
public class GiaoDichThanhToanController : ApiControllerBase
{
    private readonly ISender _sender;

    public GiaoDichThanhToanController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Ghi nhận giao dịch thanh toán theo danh sách chi tiết hóa đơn (mỗi chi tiết được thanh toán 100%)
    /// </summary>
    [HttpPost("ghi-nhan")]
    [ProducesResponseType(typeof(ApiResponse<List<int>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GhiNhan([FromBody] GhiNhanGiaoDichThanhToanCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách giao dịch thanh toán theo hóa đơn (kèm phân bổ theo chi tiết)
    /// </summary>
    [HttpPost("get-by-hoa-don")]
    [ProducesResponseType(typeof(ApiResponse<List<GiaoDichThanhToanResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByHoaDon([FromBody] GetGiaoDichThanhToanByHoaDonIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Tạo phiên thanh toán và sinh mã VietQR
    /// </summary>
    [HttpPost("tao-phien")]
    [ProducesResponseType(typeof(ApiResponse<TaoPhienThanhToanOnlineResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TaoPhien([FromBody] TaoPhienThanhToanOnlineCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Mock Webhook xác nhận thanh toán thành công (Dùng để demo)
    /// </summary>
    [AllowAnonymous] // Giả lập callback từ bên ngoài (Webhook ngân hàng)
    [HttpPost("mock-xac-nhan")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MockXacNhan([FromBody] XacNhanThanhToanOnlineCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
