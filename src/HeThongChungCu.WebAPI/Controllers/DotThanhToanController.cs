using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetDotThanhToanById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListDotThanhToan;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.CreateDotThanhToan;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.UpdateDotThanhToan;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.DeleteDotThanhToan;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;
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
    /// Tạo mới một đợt thanh toán (tháng/năm)
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<DotThanhToanDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDotThanhToanCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin đợt thanh toán (chỉ khi ở trạng thái Tạo mới)
    /// </summary>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<DotThanhToanDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateDotThanhToanCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa các đợt thanh toán (chỉ khi ở trạng thái Tạo mới)
    /// </summary>
    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteDotThanhToanCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lập hóa đơn dự thảo cho toàn bộ các dịch vụ trong đợt thanh toán
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Sau khi tạo đợt thanh toán và chốt các chỉ số tiêu thụ, quản trị viên chạy lệnh này để hệ thống tự động tính toán tiền phí cho từng căn hộ.
    /// - **Hệ thống xử lý**: 
    ///     - Quét toàn bộ căn hộ và các dịch vụ đang sử dụng.
    ///     - Tính toán phí dựa trên bảng giá đang hiệu lực.
    ///     - Tạo các bản ghi `HoaDon` và `ChiTietHoaDon` ở trạng thái 'Chờ duyệt'.
    /// </remarks>
    [HttpPost("lap-hoa-don-du-thao")]
    [ProducesResponseType(typeof(ApiResponse<LapHoaDonDuThaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LapHoaDonDuThao([FromBody] LapHoaDonDuThaoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
