using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CanHo.Commands.CreateCanHo;
using HeThongChungCu.Application.Features.CanHo.Commands.DeleteCanHo;
using HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;
using HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/can-ho")]
public class CanHoController : ApiControllerBase
{
    private readonly ISender _sender;

    public CanHoController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tạo mới một căn hộ
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL thiết lập dữ liệu ban đầu cho các căn hộ trong tòa nhà.
    /// - **Hệ thống xử lý**: 
    ///     - Xác thực tầng (`TangId`) tồn tại.
    ///     - Kiểm tra tính duy nhất của mã căn hộ trong phạm vi tòa nhà.
    ///     - Khởi tạo trạng thái mặc định cho căn hộ mới (thường là "Trống").
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `MaCanHo`, `TenCanHo`, `DienTich`, `TangId`, `SoPhongNgu`, `SoPhongTam`, `LoaiCanHoId` (Lấy tại api/catalog/loai-can-ho-for-selector).
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CanHoDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCanHoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin căn hộ
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL cập nhật lại thông số kỹ thuật hoặc thay đổi trạng thái kinh doanh của căn hộ.
    /// - **Hệ thống xử lý**: 
    ///     - Cập nhật các trường thông tin theo yêu cầu.
    ///     - Kiểm tra tính hợp lệ của việc chuyển đổi trạng thái (`TinhTrangCanHoId`).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`, `MaCanHo`, `TenCanHo`, `DienTich`, `TangId`, `SoPhongNgu`, `SoPhongTam`, `LoaiCanHoId` (api/catalog/loai-can-ho-for-selector), `TinhTrangCanHoId` (api/catalog/tinh-trang-can-ho-for-selector).
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<CanHoDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateCanHoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa một hoặc nhiều căn hộ theo danh sách ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Loại bỏ các căn hộ nhập sai hoặc không còn thuộc quản lý.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra điều kiện xóa: Căn hộ không được có cư dân đang cư trú hoặc phương tiện đang đăng ký.
    ///     - Thực hiện xóa mềm (Soft-delete) để bảo toàn lịch sử dữ liệu nếu cần.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Ids` (Danh sách ID căn hộ).
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CanHoDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteCanHoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách căn hộ (hỗ trợ tìm kiếm, lọc theo tòa nhà, sắp xếp, phân trang)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản lý tổng thể danh sách căn hộ, phục vụ tìm kiếm nhanh hoặc báo cáo.
    /// - **Hệ thống xử lý**: 
    ///     - Truy vấn kết hợp thông tin Tòa nhà, Tầng để hiển thị đầy đủ vị trí.
    ///     - Áp dụng các bộ lọc động và cơ chế phân trang phía Server để tối ưu hiệu suất (tìm kiếm theo mã hoặc tên căn hộ).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PageNumber`, `PageSize`.
    ///     - **Tùy chọn (Filter)**: `TangId`, `LoaiCanHoId` (api/catalog/loai-can-ho-for-selector), `TinhTrangCanHoId` (api/catalog/tinh-trang-can-ho-for-selector), `Keyword`, `SortCol`, `IsAsc`.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CanHoDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetListCanHoQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết căn hộ theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem chi tiết thông số và lịch sử của một căn hộ cụ thể.
    /// - **Hệ thống xử lý**: Truy xuất thông tin căn hộ kèm theo cấu trúc phân cấp (Tầng -> Tòa nhà).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<CanHoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetCanHoByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
