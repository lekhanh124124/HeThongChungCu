using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetListYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/yeu-cau-thi-cong")]
public class YeuCauThiCongController : ApiControllerBase
{
    private readonly ISender _sender;

    public YeuCauThiCongController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tìm kiếm và phân trang yêu cầu thi công
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL hoặc cư dân xem danh sách các yêu cầu thi công.
    /// - **Hệ thống xử lý**: Lọc theo căn hộ, trạng thái hành chính (TrangThaiId), trạng thái vận hành (TrangThaiThiCongId), keyword và các khoảng ngày.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<YeuCauThiCongResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromBody] GetListYeuCauThiCongQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết yêu cầu thi công kèm nhân sự và tệp
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem chi tiết thông tin, hồ sơ kỹ thuật và danh sách thợ của một yêu cầu thi công.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromBody] GetYeuCauThiCongByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
