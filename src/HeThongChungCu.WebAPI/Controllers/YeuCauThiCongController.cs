using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetListYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CreateYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.UpdateYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.TraLaiYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.SetTienDatCoc;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.AddNhanSuThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.RemoveNhanSuThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.AddTepThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.RemoveTepThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.ApproveYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.XacNhanThuCoc;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.NghiemThuThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.HoanCoc;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CompleteYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CancelYeuCauThiCong;
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

    /// <summary>
    /// Khởi tạo yêu cầu thi công (Lưu nháp hoặc Gửi duyệt)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin, Gửi duyệt hoặc Thu hồi yêu cầu
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// BQL yêu cầu bổ sung thông tin hoặc hồ sơ
    /// </summary>
    [HttpPut("tra-lai")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Return([FromBody] TraLaiYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xác định số tiền ký quỹ cần thu
    /// </summary>
    [HttpPut("set-tien-coc")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetTienDatCoc([FromBody] SetTienDatCocCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Thêm nhân sự thi công
    /// </summary>
    [HttpPost("nhan-su")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddNhanSu([FromBody] AddNhanSuThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa nhân sự thi công (yêu cầu lý do)
    /// </summary>
    [HttpDelete("nhan-su")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveNhanSu([FromBody] RemoveNhanSuThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Bổ sung hồ sơ kỹ thuật/tệp đính kèm
    /// </summary>
    [HttpPost("tep")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddTep([FromBody] AddTepThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa tệp đính kèm
    /// </summary>
    [HttpDelete("tep")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveTep([FromBody] RemoveTepThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Phê duyệt chính thức (Chuyển sang bước thu cọc)
    /// </summary>
    [HttpPut("approve")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve([FromBody] ApproveYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xác nhận đã thu tiền ký quỹ
    /// </summary>
    [HttpPut("thu-coc")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> XacNhanThuCoc([FromBody] XacNhanThuCocCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xác nhận hoàn tất thi công về mặt kỹ thuật
    /// </summary>
    [HttpPut("nghiem-thu")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> NghiemThu([FromBody] NghiemThuThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Quyết toán tiền cọc (có khấu trừ hư hại nếu có)
    /// </summary>
    [HttpPut("hoan-coc")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> HoanCoc([FromBody] HoanCocCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Đóng hồ sơ yêu cầu thi công
    /// </summary>
    [HttpPut("complete")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Complete([FromBody] CompleteYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Hủy yêu cầu thi công (Nhập lý do)
    /// </summary>
    [HttpDelete("cancel")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauThiCongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel([FromBody] CancelYeuCauThiCongCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
