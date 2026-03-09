using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QuanHeCuTru.Commands.CapNhatQuanHe;
using HeThongChungCu.Application.Features.QuanHeCuTru.Commands.KetThucCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.Commands.ThietLapCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByUsername;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/quan-he-cu-tru")]
public class QuanHeCuTruController : ApiControllerBase
{
    private readonly ISender _sender;

    public QuanHeCuTruController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Thiết lập cư trú – thêm cư dân vào căn hộ
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CuDanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ThietLapCuTru([FromBody] ThietLapCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Kết thúc cư trú – đánh dấu cư dân đã chuyển đi
    /// </summary>
    [HttpPut("ket-thuc")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> KetThucCuTru([FromBody] KetThucCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật quan hệ cư trú (loại quan hệ trong căn hộ)
    /// </summary>
    [HttpPut("cap-nhat")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapNhatQuanHe([FromBody] CapNhatQuanHeCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách cư dân đang sống tại một căn hộ
    /// </summary>
    [HttpPost("cu-dan")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CuDanResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LayCuDan([FromBody] LayCuDanByCanHoIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy lịch sử cư trú theo căn hộ hoặc theo cư dân (phải truyền CanHoId hoặc UserId)
    /// </summary>
    [HttpPost("lich-su")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<LichSuCuTruResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LayLichSuCuTru([FromBody] LayLichSuCuTruQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Tìm kiếm người dùng theo username (chỉ Resident hoặc Guest)
    /// </summary>
    [HttpPost("search-user")]
    [ProducesResponseType(typeof(ApiResponse<SearchUserByUsernameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TimKiemNguoiDung([FromBody] GetUserByUsernameQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
