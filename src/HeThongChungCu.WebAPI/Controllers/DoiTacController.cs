using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHopDong;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Commands.RevokeHopDong;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/doi-tac")]
public class DoiTacController : ApiControllerBase
{
    private readonly ISender _sender;

    public DoiTacController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tạo mới một đối tác (đơn vị cung cấp)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi BQL muốn thêm một đơn vị mới vào hệ thống để quản lý các hợp đồng và dịch vụ đi kèm.
    /// - **Hệ thống xử lý**: 
    ///     - Lưu trữ thông tin cơ bản: Tên, địa chỉ, mã số thuế, thông tin liên hệ.
    ///     - Khởi tạo danh sách hợp đồng trống cho đối tác này.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `TenDoiTac`, `DiaChi`, `MaSoThue`, `NguoiDaiDien`, `SoDienThoai`.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DoiTacDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDoiTacCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin đối tác
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi đối tác thay đổi thông tin liên lạc, địa chỉ hoặc người đại diện.
    /// - **Hệ thống xử lý**: Cập nhật thông tin chi tiết của đối tác vào cơ sở dữ liệu.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`, `TenDoiTac`, `DiaChi`, `MaSoThue`, `NguoiDaiDien`, `SoDienThoai`.
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<DoiTacDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateDoiTacCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Ký hợp đồng mới với đối tác
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi cần bổ sung một dịch vụ mới từ đối tác hiện tại.
    /// - **Hệ thống xử lý**: 
    ///     - Tạo mới một dịch vụ (`DichVu`) tương ứng với các thông số truyền vào.
    ///     - Tạo mới một hợp đồng (`HopDongDoiTac`) liên kết với đối tác và dịch vụ vừa tạo.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `DoiTacId`, `HopDong.SoHopDong`, `HopDong.MaDichVu`, `HopDong.TenDichVu`.
    /// </remarks>
    [HttpPost("hop-dong")]
    [ProducesResponseType(typeof(ApiResponse<DoiTacDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateHopDong([FromBody] CreateHopDongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa danh sách đối tác theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Loại bỏ các đối tác không còn hợp tác hoặc nhập sai thông tin.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra các ràng buộc: Đối tác không được có hợp đồng đang còn hiệu lực để đảm bảo tính toàn vẹn.
    ///     - Xóa các bản ghi đối tác theo danh sách IDs.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Ids` (Danh sách ID đối tác).
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteDoiTacsCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Thu hồi/Thanh lý các hợp đồng đối tác (Bulk Revoke)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi hợp đồng kết thúc trước hạn hoặc không còn hiệu lực.
    /// - **Hệ thống xử lý**: Chuyển trạng thái hợp đồng sang `Đã thanh lý` và cập nhật ngày kết thúc.
    /// </remarks>
    [HttpDelete("hop-dong/revoke")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeHopDong([FromBody] RevokeHopDongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult<bool>(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách đối tác (hỗ trợ phân trang, lọc, tìm kiếm)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Trang quản lý danh sách đối tác tổng thể.
    /// - **Hệ thống xử lý**: 
    ///     - Tìm kiếm theo `Keyword` (Tên đối tác, mã số thuế).
    ///     - Áp dụng phân trang và sắp xếp.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PageNumber`, `PageSize`.
    ///     - **Tùy chọn**: `Keyword`, `SortCol`, `IsAsc`.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DoiTacResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetListDoiTacsQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết đối tác theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem thông tin đầy đủ của một đối tác bao gồm cả danh sách hợp đồng liên quan.
    /// - **Hệ thống xử lý**: Truy xuất thông tin chi tiết của đối tác từ database.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<DoiTacDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetDoiTacByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
