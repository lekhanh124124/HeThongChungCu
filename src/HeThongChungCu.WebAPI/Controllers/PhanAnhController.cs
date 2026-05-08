using HeThongChungCu.Application.Features.QLPhanAnh.Commands.CreatePhanAnh;
using HeThongChungCu.Application.Features.QLPhanAnh.Commands.UpdatePhanAnh;
using HeThongChungCu.Application.Features.QLPhanAnh.Commands.HuyPhanAnh;
using HeThongChungCu.Application.Features.QLPhanAnh.Commands.TiepNhanVaPhanCong;
using HeThongChungCu.Application.Features.QLPhanAnh.Commands.SubmitTraLoiPhanAnh;
using HeThongChungCu.Application.Features.QLPhanAnh.Commands.XacNhanHoanThanhPhanAnh;
using HeThongChungCu.Application.Features.QLPhanAnh.Commands.CuDanDanhGiaVaDongTicket;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhList;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/phan-anh")]
public class PhanAnhController : ApiControllerBase
{
    private readonly ISender _sender;

    public PhanAnhController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tìm kiếm và phân trang các phản ánh khiếu nại của cư dân
    /// </summary>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PhanAnhResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromBody] GetPhanAnhListQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết thông tin phản ánh kèm tệp đính kèm và lịch sử trò chuyện
    /// </summary>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<PhanAnhDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromBody] GetPhanAnhByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Cư dân gửi phản ánh khiếu nại mới lên ban quản lý (hỗ trợ Lưu nháp hoặc Gửi đi trực tiếp)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PhanAnhResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreatePhanAnhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cư dân chỉnh sửa phản ánh nháp, thu hồi phản ánh hoặc gửi đi phản ánh đã lưu nháp
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<PhanAnhResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdatePhanAnhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// BQL tiếp nhận phản ánh và hệ thống tự động gán tài khoản đăng nhập hiện tại làm người xử lý
    /// </summary>
    [HttpPut("tiep-nhan")]
    [ProducesResponseType(typeof(ApiResponse<PhanAnhResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TiepNhanPhanCong([FromBody] TiepNhanVaPhanCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Gửi câu trả lời, phản hồi trao đổi trong luồng chat của phản ánh
    /// </summary>
    [HttpPost("submit-tra-loi")]
    [ProducesResponseType(typeof(ApiResponse<PhanAnhResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitTraLoi([FromBody] SubmitTraLoiPhanAnhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Nhân viên trực tiếp nhận báo cáo đã xử lý hoàn tất phản ánh (hệ thống tự động ghi nhận người xử lý hiện tại)
    /// </summary>
    [HttpPut("xac-nhan-hoan-thanh")]
    [ProducesResponseType(typeof(ApiResponse<PhanAnhResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> XacNhanHoanThanh([FromBody] XacNhanHoanThanhPhanAnhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// BQL hoặc người có thẩm quyền thực hiện hủy/từ chối phản ánh của cư dân kèm theo lý do cụ thể
    /// </summary>
    [HttpPut("huy")]
    [ProducesResponseType(typeof(ApiResponse<PhanAnhResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Huy([FromBody] HuyPhanAnhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cư dân chấm điểm sao, viết nhận xét và chính thức đóng ticket
    /// </summary>
    [HttpPut("danh-gia")]
    [ProducesResponseType(typeof(ApiResponse<PhanAnhResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CuDanDanhGiaDongTicket([FromBody] CuDanDanhGiaVaDongTicketCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
