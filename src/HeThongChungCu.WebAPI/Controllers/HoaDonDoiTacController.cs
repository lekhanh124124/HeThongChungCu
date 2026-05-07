using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.XacNhanThanhToanDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetHoaDonDoiTacById;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/hoa-don-doi-tac")]
public class HoaDonDoiTacController : ApiControllerBase
{
    private readonly ISender _sender;

    public HoaDonDoiTacController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lập hóa đơn đối tác mới (Chi trả nhà cung cấp)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi Ban quản lý nhận được hóa đơn yêu cầu thanh toán từ đơn vị cung cấp theo chu kỳ hợp đồng.
    /// - **Hệ thống xử lý**:
    ///     - Xác thực hợp đồng đối tác phải đang hoạt động.
    ///     - Chặn lập trùng hóa đơn cho cùng một kỳ (Tháng/Năm) của hợp đồng này.
    ///     - Kích hoạt chứng từ đính kèm (MarkAsUsed) nếu có.
    ///     - Khởi tạo hóa đơn ở trạng thái `Chưa thanh toán`.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<HoaDonDoiTacResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateHoaDonDoiTacCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin hóa đơn đối tác chưa thanh toán
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi hóa đơn bị sai sót thông tin về số tiền, kỳ thanh toán hoặc ghi chú trước khi thanh toán.
    /// - **Hệ thống xử lý**:
    ///     - Chỉ cho phép sửa đổi khi hóa đơn ở trạng thái `Chưa thanh toán`.
    ///     - Kiểm tra chặn trùng kỳ hóa đơn (Tháng/Năm) mới.
    ///     - Giải phóng chứng từ cũ (MarkAsUnused) và kích hoạt chứng từ mới (MarkAsUsed).
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<HoaDonDoiTacResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateHoaDonDoiTacCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa hóa đơn đối tác chưa thanh toán
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi lập sai hóa đơn hoặc đối tác hủy yêu cầu thanh toán.
    /// - **Hệ thống xử lý**:
    ///     - Chỉ cho phép xóa khi hóa đơn ở trạng thái `Chưa thanh toán`.
    ///     - Giải phóng chứng từ đính kèm (MarkAsUnused) để hệ thống tự động dọn dẹp.
    ///     - Thực hiện xóa mềm hóa đơn đối tác.
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteHoaDonDoiTacCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xác nhận thanh toán hóa đơn đối tác
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi kế toán hoặc thủ quỹ đã thực hiện chuyển tiền thành công cho nhà cung cấp và có biên nhận chuyển khoản.
    /// - **Hệ thống xử lý**: Chuyển trạng thái hóa đơn đối tác từ `Chưa thanh toán` sang `Đã thanh toán`. Tác vụ này mang tính chất Idempotent (gửi lại nhiều lần vẫn an toàn).
    /// </remarks>
    [HttpPut("xac-nhan-thanh-toan")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> XacNhanThanhToan([FromBody] XacNhanThanhToanDoiTacCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách hóa đơn đối tác (Phân trang, Lọc và Tìm kiếm)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Trang quản trị danh sách hóa đơn đối tác cho Ban quản lý và kế toán.
    /// - **Bộ lọc hỗ trợ**: Lọc theo đối tác (`DoiTacId`), hợp đồng (`HopDongDoiTacId`), Tháng, Năm, Trạng thái thanh toán.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<HoaDonDoiTacResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetListHoaDonDoiTacQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết hóa đơn đối tác theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem chi tiết hóa đơn đối tác, thông tin đối tác ký kết và thông tin người lập/chỉnh sửa hóa đơn.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<HoaDonDoiTacDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetHoaDonDoiTacByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
