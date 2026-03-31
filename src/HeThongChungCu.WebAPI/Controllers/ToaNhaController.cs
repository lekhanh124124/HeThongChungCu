using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ToaNha.Commands.CreateToaNha;
using HeThongChungCu.Application.Features.ToaNha.Commands.DeleteToaNha;
using HeThongChungCu.Application.Features.ToaNha.Commands.UpdateToaNha;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;
using HeThongChungCu.Application.Features.ToaNha.Queries.GetToaNhaById;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/toa-nha")]
public class ToaNhaController : ApiControllerBase
{
    private readonly ISender _sender;

    public ToaNhaController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tạo mới một tòa nhà
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản trị viên hệ thống thiết lập thông tin cơ sở hạ tầng ban đầu cho các tòa nhà trong dự án.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra tính duy nhất của mã tòa nhà (`MaToaNha`) trong toàn hệ thống.
    ///     - Lưu trữ địa chỉ và thông tin mô tả chi tiết của tòa nhà.
    ///     - Thiết lập trạng thái hoạt động mặc định.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `MaToaNha`, `TenToaNha`, `DiaChi`.
    ///     - **Tùy chọn**: `MoTa`.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin tòa nhà
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL cập nhật tên tòa nhà, địa chỉ hoặc thay đổi trạng thái vận hành (ví dụ: Tạm dừng hoạt động).
    /// - **Hệ thống xử lý**: 
    ///     - Cập nhật thông tin vào cơ sở dữ liệu.
    ///     - Ghi nhận trạng thái mới (`TrangThaiToaNhaId`) tác động đến hiển thị và quản lý các tầng/căn hộ thuộc tòa nhà này.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`, `MaToaNha`, `TenToaNha`, `DiaChi`, `TrangThaiToaNhaId` (Lấy tại api/catalog/trang-thai-toa-nha-for-selector).
    ///     - **Tùy chọn**: `MoTa`.
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa một hoặc nhiều tòa nhà theo danh sách ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Loại bỏ các tòa nhà nhập sai hoặc không còn thuộc phạm vi quản lý của dự án.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra ràng buộc dữ liệu: Tòa nhà không được chứa bất kỳ tầng nào để đảm bảo tính toàn vẹn hệ thống.
    ///     - Xóa các bản ghi tòa nhà theo danh sách IDs cung cấp.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Ids` (Danh sách ID tòa nhà).
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ToaNhaDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách tòa nhà (hỗ trợ tìm kiếm, lọc, sắp xếp, phân trang)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản lý tổng thể danh sách tòa nhà, tra cứu nhanh thông tin vị trí và trạng thái.
    /// - **Hệ thống xử lý**: 
    ///     - Truy vấn danh sách tòa nhà kèm theo các bộ lọc trạng thái và từ khóa tìm kiếm (theo mã hoặc tên tòa nhà).
    ///     - Áp dụng phân trang và sắp xếp linh hoạt theo yêu cầu từ giao diện.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PageNumber`, `PageSize`.
    ///     - **Tùy chọn (Filter)**: `Keyword`, `TrangThaiToaNhaId` (api/catalog/trang-thai-toa-nha-for-selector), `SortCol`, `IsAsc`.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ToaNhaDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetListToaNhaQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết tòa nhà theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem thông tin cấu hình chi tiết và các thuộc tính liên quan của một tòa nhà cụ thể.
    /// - **Hệ thống xử lý**: Truy xuất bản ghi tòa nhà và trả về thông tin chi tiết đầy đủ.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetToaNhaByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
