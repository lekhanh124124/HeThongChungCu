using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/hoa-don")]
public class HoaDonController : ApiControllerBase
{
    private readonly ISender _sender;

    public HoaDonController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách hóa đơn (hỗ trợ phân trang, lọc theo căn hộ, đợt thanh toán, trạng thái, tháng, năm)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản lý tổng thể các hóa đơn trong hệ thống, tra cứu lịch sử thanh toán của căn hộ.
    /// - **Hệ thống xử lý**: 
    ///     - Lọc theo `CanHoId`, `DotThanhToanId`, `TrangThaiHoaDonId`, `Thang`, `Nam`.
    ///     - Tìm kiếm theo `Keyword` (Mã hóa đơn).
    ///     - Hỗ trợ phân trang mặc định (10 bản ghi/trang).
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<HoaDonResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetListHoaDonQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết hóa đơn theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem thông tin chi tiết các khoản phí trong một hóa đơn.
    /// - **Hệ thống xử lý**: Truy xuất bản ghi hóa đơn và danh sách chi tiết các mục phí đi kèm.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<HoaDonDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetHoaDonByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
