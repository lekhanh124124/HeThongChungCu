using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;
using HeThongChungCu.Application.Features.QLThanhToan.Commands.PhatHanhHoaDon;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietCoDinh;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietLuyTien;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietDienTich;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietKhungGio;
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
    
    /// <summary>
    /// Phát hành hóa đơn (chuyển từ trạng thái Chờ duyệt sang Chưa thanh toán)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản trị viên phê duyệt và chính thức phát hành hóa đơn để cư dân có thể nhìn thấy và thanh toán.
    /// - **Hệ thống xử lý**: 
    ///     - Nếu `HoaDonIds` có giá trị: Chỉ phát hành các hóa đơn được chỉ định.
    ///     - Nếu `HoaDonIds` rỗng: Phát hành toàn bộ hóa đơn đang ở trạng thái 'Chờ duyệt' trong đợt thanh toán.
    ///     - Cập nhật trạng thái hóa đơn và ghi nhận lịch sử (nếu có).
    /// </remarks>
    [HttpPost("phat-hanh")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PhatHanh([FromBody] PhatHanhHoaDonCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết giá cố định
    /// </summary>
    [HttpPost("get-chi-tiet-co-dinh")]
    [ProducesResponseType(typeof(ApiResponse<ChiTietCoDinhResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChiTietCoDinh([FromBody] GetChiTietCoDinhQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết giá lũy tiến
    /// </summary>
    [HttpPost("get-chi-tiet-luy-tien")]
    [ProducesResponseType(typeof(ApiResponse<ChiTietLuyTienResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChiTietLuyTien([FromBody] GetChiTietLuyTienQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết giá theo diện tích
    /// </summary>
    [HttpPost("get-chi-tiet-dien-tich")]
    [ProducesResponseType(typeof(ApiResponse<ChiTietDienTichResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChiTietDienTich([FromBody] GetChiTietDienTichQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết giá theo khung giờ
    /// </summary>
    [HttpPost("get-chi-tiet-khung-gio")]
    [ProducesResponseType(typeof(ApiResponse<ChiTietKhungGioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChiTietKhungGio([FromBody] GetChiTietKhungGioQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
