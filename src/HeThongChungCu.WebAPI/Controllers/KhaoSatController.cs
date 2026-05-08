using HeThongChungCu.Application.Features.QLKhaoSat.Commands.CreateKhaoSat;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.GuiOtpBieuQuyet;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.XacNhanBieuQuyet;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.PublishKhaoSat;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.UpdateKhaoSat;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.DeleteKhaoSat;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatList;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKetQuaKhaoSat;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
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
[Route("api/khao-sat")]
public class KhaoSatController : ApiControllerBase
{
    private readonly ISender _sender;

    public KhaoSatController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tìm kiếm và phân trang danh sách các đợt khảo sát/bầu cử Ban quản trị
    /// </summary>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<KhaoSatResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromBody] GetKhaoSatListQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết câu hỏi và các lựa chọn đáp án của một đợt khảo sát
    /// </summary>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<KhaoSatDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromBody] GetKhaoSatByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Xem báo cáo thống kê tỷ lệ phần trăm đồng ý, số căn tham gia, và tổng trọng số m2 diện tích
    /// </summary>
    [HttpPost("get-ket-qua")]
    [ProducesResponseType(typeof(ApiResponse<KetQuaKhaoSatResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKetQua([FromBody] GetKetQuaKhaoSatQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Khởi tạo chiến dịch khảo sát hoặc đợt bầu cử Ban Quản trị chung cư mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<KhaoSatResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateKhaoSatCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin và câu hỏi khảo sát (chỉ khi đang ở trạng thái Nháp - Draft)
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<KhaoSatResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateKhaoSatCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Chính thức công bố / phát hành đợt khảo sát để cư dân bắt đầu bỏ phiếu
    /// </summary>
    [HttpPut("cong-bo")]
    [ProducesResponseType(typeof(ApiResponse<KhaoSatResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Publish([FromBody] PublishKhaoSatCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa đợt khảo sát (chỉ khi đang ở trạng thái Nháp - Draft)
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromBody] DeleteKhaoSatCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cư dân yêu cầu nhận mã OTP xác minh biểu quyết qua Email đăng ký
    /// </summary>
    [HttpPost("gui-otp-bieu-quyet")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GuiOtpBieuQuyet([FromBody] GuiOtpBieuQuyetCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xác thực OTP, chống trùng lặp, tính trọng số và nộp phiếu bầu chính thức
    /// </summary>
    [HttpPost("xac-nhan-bieu-quyet")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> XacNhanBieuQuyet([FromBody] XacNhanBieuQuyetCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
