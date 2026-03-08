using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ChungCu.Commands.CreateToaNha;
using HeThongChungCu.Application.Features.ChungCu.Commands.DeleteToaNha;
using HeThongChungCu.Application.Features.ChungCu.Commands.UpdateToaNha;
using HeThongChungCu.Application.Features.ChungCu.Commands.CreateCanHo;
using HeThongChungCu.Application.Features.ChungCu.Commands.UpdateCanHo;
using HeThongChungCu.Application.Features.ChungCu.Commands.DeleteCanHo;
using HeThongChungCu.Application.Features.ChungCu.DTOs;
using HeThongChungCu.Application.Features.ChungCu.Queries.GetAllToaNhas;
using HeThongChungCu.Application.Features.ChungCu.Queries.GetToaNhaById;
using HeThongChungCu.Application.Features.ChungCu.Queries.GetAllCanHos;
using HeThongChungCu.Application.Features.ChungCu.Queries.GetCanHoById;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/chung-cu")]
public class ChungCuController : ApiControllerBase
{
    private readonly ISender _sender;

    public ChungCuController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tạo mới một tòa nhà
    /// </summary>
    [HttpPost("toa-nha")]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin tòa nhà
    /// </summary>
    [HttpPut("toa-nha")]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa một hoặc nhiều tòa nhà theo danh sách ID
    /// </summary>
    [HttpDelete("toa-nha")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ToaNhaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách tòa nhà (hỗ trợ tìm kiếm, lọc, sắp xếp, phân trang)
    /// </summary>
    [HttpPost("toa-nha/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ToaNhaDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetAllToaNhasQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết tòa nhà theo ID
    /// </summary>
    [HttpPost("toa-nha/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetToaNhaByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    // ───────────────────────── CĂN HỘ ─────────────────────────

    /// <summary>
    /// Tạo mới một căn hộ
    /// </summary>
    [HttpPost("can-ho")]
    [ProducesResponseType(typeof(ApiResponse<CanHoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCanHo([FromBody] CreateCanHoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin căn hộ
    /// </summary>
    [HttpPut("can-ho")]
    [ProducesResponseType(typeof(ApiResponse<CanHoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCanHo([FromBody] UpdateCanHoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa một hoặc nhiều căn hộ theo danh sách ID
    /// </summary>
    [HttpDelete("can-ho")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CanHoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCanHo([FromBody] DeleteCanHoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách căn hộ (hỗ trợ tìm kiếm, lọc theo tòa nhà, sắp xếp, phân trang)
    /// </summary>
    [HttpPost("can-ho/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CanHoDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCanHoList([FromBody] GetAllCanHosQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết căn hộ theo ID
    /// </summary>
    [HttpPost("can-ho/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<CanHoDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCanHoById([FromBody] GetCanHoByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
