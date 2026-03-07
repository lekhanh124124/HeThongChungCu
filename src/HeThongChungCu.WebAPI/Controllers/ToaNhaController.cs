using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ChungCu.Commands.CreateToaNha;
using HeThongChungCu.Application.Features.ChungCu.Commands.DeleteToaNha;
using HeThongChungCu.Application.Features.ChungCu.Commands.UpdateToaNha;
using HeThongChungCu.Application.Features.ChungCu.DTOs;
using HeThongChungCu.Application.Features.ChungCu.Queries.GetAllToaNhas;
using HeThongChungCu.Application.Features.ChungCu.Queries.GetToaNhaById;
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
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin tòa nhà
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa một hoặc nhiều tòa nhà theo danh sách ID
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ToaNhaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteToaNhaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách tòa nhà (hỗ trợ tìm kiếm, lọc, sắp xếp, phân trang)
    /// </summary>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ToaNhaDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetAllToaNhasQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết tòa nhà theo ID
    /// </summary>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<ToaNhaDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetToaNhaByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
