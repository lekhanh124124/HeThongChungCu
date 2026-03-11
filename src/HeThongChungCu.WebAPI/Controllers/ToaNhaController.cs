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
    /// API dùng đề đăng ký một tòa nhà mới vào hệ thống quản lý.
    /// Yêu cầu cung cấp `MaToaNha`, `TenToaNha`, `DiaChi`, `MoTa` và `TrangThaiToaNhaId`.
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
    /// Chỉnh sửa các thông tin hiện tại của một tòa nhà đã tồn tại. Yêu cầu truyền `Id` của tòa nhà.
    /// Trả về chi tiết Tòa nhà sau khi đã được cập nhật thành công.
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
    /// API cho phép xóa (dạng soft-delete) nhiều tòa nhà cùng một lúc.
    /// Nhận vào danh sách `Ids`. Không thể xóa tòa nhà nếu bên trong vẫn còn Căn hộ đang hoạt động.
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
    /// Truy vấn danh sách tòa nhà kèm theo bộ lọc:
    /// - Tìm kiếm theo Tên hoặc Mã tòa nhà (`SearchTerm`).
    /// - Lọc theo Trạng thái hoạt động (`TrangThaiToaNhaId`).
    /// - Hỗ trợ phân trang, sắp xếp.
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
    /// Trả về toàn bộ thông tin chi tiết của một Tòa nhà cụ thể dựa vào `Id`.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetToaNhaByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
