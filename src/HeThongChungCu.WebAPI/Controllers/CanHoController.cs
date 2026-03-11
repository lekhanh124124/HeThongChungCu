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
    /// API dùng để thêm một căn hộ mới vào một tòa nhà cụ thể.
    /// Yêu cầu cung cấp đầy đủ thông tin: MaCanHo, DienTich, Tang, SoPhongNgu, SoPhongTam, LoaiCanHoId.
    /// Trả về chi tiết Căn hộ vừa được tạo.
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
    /// API dùng để chỉnh sửa thông tin của một căn hộ đã tồn tại.
    /// Yêu cầu truyền `Id` của căn hộ và các thông tin cần cập nhật.
    /// Trả về thông tin Căn hộ sau khi đã cập nhật thành công.
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
    /// API cho phép xóa (soft-delete) một danh sách các căn hộ.
    /// Truyền vào danh sách `Ids`. Trả về danh sách thông tin các căn hộ vừa bị xóa.
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
    /// API dùng để truy vấn danh sách căn hộ theo nhiều tiêu chí:
    /// - Lọc theo: `TangId`, `LoaiCanHoId`, `TinhTrangCanHoId`.
    /// - Tìm kiếm theo `SearchTerm` (mã căn hộ).
    /// - Hỗ trợ phân trang (`PageNumber`, `PageSize`) và sắp xếp (`OrderBy`).
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
    /// API lấy thông tin chi tiết đầy đủ của một căn hộ cụ thể thông qua `Id`.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<CanHoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetCanHoByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
