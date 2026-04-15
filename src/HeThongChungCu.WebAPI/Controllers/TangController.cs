using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Tang.Commands.CreateTang;
using HeThongChungCu.Application.Features.Tang.Commands.DeleteTang;
using HeThongChungCu.Application.Features.Tang.Commands.UpdateTang;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Application.Features.Tang.Queries.GetListTang;
using HeThongChungCu.Application.Features.Tang.Queries.GetTangById;
using HeThongChungCu.Application.Features.Tang.Queries.GoiYMaTang;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/tang")]
public class TangController : ApiControllerBase
{
    private readonly ISender _sender;

    public TangController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gợi ý mã tầng mới
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi quản trị viên đang tạo mới tầng và muốn hệ thống gợi ý một mã tầng theo quy tắc increment cho tòa nhà đó.
    /// - **Hệ thống xử lý**: Tìm tòa nhà và lấy số lượng tầng theo loại (hầm/nổi) để gợi ý mã (F... hoặc B...).
    /// </remarks>
    [HttpPost("goi-y-ma-tang")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GoiYMaTang([FromBody] GoiYMaTangQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Tạo mới một tầng
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL thiết lập phân cấp hạ tầng cho tòa nhà bằng cách thêm các tầng.
    /// - **Hệ thống xử lý**: 
    ///     - Xác thực tòa nhà (`ToaNhaId`) tồn tại.
    ///     - Kiểm tra tính duy nhất của mã tầng trong cùng một tòa nhà.
    ///     - Phân loại tầng dựa trên `LoaiTangId` (Tầng ở, Tầng kỹ thuật, Hầm, v.v.).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `MaTang`, `TenTang`, `ToaNhaId`, `LoaiTangId` (Lấy tại api/catalog/loai-tang-for-selector).
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TangDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTangCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin tầng
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL thay đổi tên gọi, mã định danh hoặc chức năng của tầng.
    /// - **Hệ thống xử lý**: 
    ///     - Cập nhật các thông tin cơ bản của tầng.
    ///     - Đảm bảo tính nhất quán với tòa nhà chủ quản.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`, `MaTang`, `TenTang`, `ToaNhaId`, `LoaiTangId` (Lấy tại api/catalog/loai-tang-for-selector).
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<TangDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateTangCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa tầng theo sách ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xóa các tầng nhập sai hoặc hạ tầng không còn tồn tại.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra ràng buộc: Tầng không được chứa bất kỳ căn hộ nào để đảm bảo tính toàn vẹn dữ liệu.
    ///     - Thực hiện xóa các bản ghi tầng tương ứng.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Ids` (Danh sách ID tầng).
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TangDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteTangCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách tầng
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Duyệt danh sách hạ tầng tầng của tòa nhà phục vụ quản lý hoặc chọn lọc.
    /// - **Hệ thống xử lý**: 
    ///     - Truy vấn danh sách tầng kèm theo thông tin tòa nhà.
    ///     - Hỗ trợ lọc theo loại tầng và tìm kiếm theo tên/mã (qua Keyword).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PageNumber`, `PageSize`.
    ///     - **Tùy chọn (Filter)**: `ToaNhaId`, `LoaiTangId` (api/catalog/loai-tang-for-selector), `Keyword`, `SortCol`, `IsAsc`.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TangDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetListTangQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết tầng theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem cấu hình chi tiết của một tầng nhất định.
    /// - **Hệ thống xử lý**: Truy xuất thông tin tầng và các thuộc tính liên quan.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<TangResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetTangByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
