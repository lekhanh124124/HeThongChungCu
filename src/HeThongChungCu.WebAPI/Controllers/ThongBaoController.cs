using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ThongBao.Commands.DanhDauDaDoc;
using HeThongChungCu.Application.Features.ThongBao.DTOs;
using HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/thong-bao")]
public class ThongBaoController : ApiControllerBase
{
    private readonly ISender _sender;

    public ThongBaoController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách thông báo của người dùng hiện tại (Phân trang, lọc)
    /// </summary>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ThongBaoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromBody] LayDSThongBaoQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Đánh dấu một thông báo là đã đọc
    /// </summary>
    [HttpPut("da-doc")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead([FromBody] DanhDauDaDocCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
