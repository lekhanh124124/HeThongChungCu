using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatThongTinPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.KichHoatPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.HuyPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.DangKyPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaThePhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.BaoMatThePhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoThePhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatYeuCauPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GoiYMaThePhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSPhuongTienTrongChungCu;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[Route("api/phuong-tien")]
[ApiController]
public class PhuongTienController : ApiControllerBase
{
    private readonly ISender _sender;

    public PhuongTienController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách phương tiện trong chung cư với bộ lọc và tìm kiếm nâng cao
    /// </summary>
    /// <remarks>
    /// API truy vấn danh sách phương tiện hỗ trợ các chức năng:
    /// - **Phạm vi (ID)**: Lọc chính xác theo ToaNhaId, TangId, CanHoId.
    /// - **Từ khóa**: Tìm kiếm theo TenPhuongTien, BienSo, MauXe (qua tham số Keyword).
    /// - **Bộ lọc**: 
    ///     - Mã định danh: MaToaNha, MaTang, MaCanHo.
    ///     - Thông tin xe: LoaiPhuongTienId, MauXe, TrangThaiPhuongTienId.
    /// - **Sắp xếp**: Hỗ trợ sắp xếp theo MaToaNha, MaTang, MaCanHo, TenPhuongTien, BienSo, MauXe, TrangThaiPhuongTienId.
    /// - **Phân trang**: PageNumber và PageSize.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PhuongTienResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromBody] LayDSPhuongTienTrongChungCuQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết phương tiện cùng danh sách thẻ
    /// </summary>
    /// <param name="query">Dữ liệu yêu cầu (Id)</param>
    /// <param name="cancellationToken">Token hủy bỏ tác vụ</param>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<PhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromBody] GetPhuongTienByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Đăng ký phương tiện mới cho căn hộ
    /// </summary>
    /// <remarks>
    /// API dùng để tạo một bản ghi phương tiện mới thuộc về một `CanHo`.
    /// Yêu cầu cung cấp `CanHoId`, `TenPhuongTien`, ID loại phương tiện (`LoaiPhuongTienId`), `BienSo`, `MauXe`, `HinhAnhPhuongTiens`.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DangKyPhuongTien(
        [FromBody] DangKyPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cập nhật thông tin phương tiện
    /// </summary>
    /// <remarks>
    /// API dùng để sửa thông tin cơ bản của phương tiện như tên, loại, biển số, màu xe, hình ảnh.
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<PhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapNhatThongTinPhuongTien(
        [FromBody] CapNhatThongTinPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kích hoạt một hoặc nhiều phương tiện
    /// </summary>
    /// <remarks>
    /// API dùng để kích hoạt lại phương tiện sang trạng thái Active. Kiểm tra hạn mức căn hộ trước khi kích hoạt.
    /// </remarks>
    [HttpPut("kich-hoat")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> KichHoatPhuongTien(
        [FromBody] KichHoatPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Hủy một hoặc nhiều phương tiện
    /// </summary>
    /// <remarks>
    /// API dùng để chuyển trạng thái phương tiện sang Inactive và khóa tất cả thẻ liên quan.
    /// Các phương tiện đã bị hủy sẽ không thể kích hoạt lại.
    /// </remarks>
    [HttpPut("huy")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HuyPhuongTien(
        [FromBody] HuyPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Khóa một hoặc nhiều phương tiện
    /// </summary>
    /// <remarks>
    /// API dùng để chuyển trạng thái phương tiện sang Blocked và khóa tất cả thẻ liên quan.
    /// </remarks>
    [HttpPut("khoa")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> KhoaPhuongTien(
        [FromBody] KhoaPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Tạo thẻ phương tiện (gán mã thẻ cho phương tiện)
    /// </summary>
    /// <remarks>
    /// API dùng để gán một mã thẻ (`MaThe`) cho một phương tiện (`PhuongTienId`) đã có.
    /// </remarks>
    [HttpPost("the-phuong-tien")]
    [ProducesResponseType(typeof(ApiResponse<ThePhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TaoThePhuongTien(
        [FromBody] TaoThePhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Gợi ý mã thẻ phương tiện
    /// </summary>
    /// <remarks>
    /// API dùng để gợi ý một mã thẻ mới dựa trên `PhuongTienId` và ID thẻ cuối cùng.
    /// Quy tắc: `CARD-V-{PhuongTienId:D4}{last ThePhuongTienId + 1 : D4}`
    /// Các số 0 padding sẽ được thay bằng số ngẫu nhiên.
    /// </remarks>
    [HttpPost("the-phuong-tien/goi-y-ma-the")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoiYMaThe(
        [FromBody] GoiYMaThePhuongTienQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Khóa thẻ phương tiện 
    /// Các thẻ đã bị khóa sẽ không thể sử dụng.
    /// </summary>
    [HttpPut("the-phuong-tien/khoa")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> KhoaThe(
        [FromBody] KhoaThePhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Báo mất thẻ phương tiện (Dành cho cư dân)
    /// </summary>
    /// <remarks>
    /// API cho phép cư dân báo mất thẻ xe của mình. Hệ thống sẽ khóa thẻ và ghi nhận trạng thái mất.
    /// Yêu cầu: Người dùng phải là cư dân đang cư trú hợp pháp tại căn hộ sở hữu phương tiện.
    /// </remarks>
    [HttpPut("the-phuong-tien/bao-mat")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BaoMatThe(
        [FromBody] BaoMatThePhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Tạo yêu cầu về phương tiện (Thêm, Sửa, Xóa) - Dành cho cư dân
    /// </summary>
    /// <remarks>
    /// API cho phép cư dân tạo các yêu cầu liên quan đến phương tiện của mình:
    /// - **Thêm (LoaiYeuCauId = 1)**: Đăng ký xe mới cho căn hộ.
    /// - **Sửa (LoaiYeuCauId = 2)**: Cập nhật thông tin xe hiện có (yêu cầu `PhuongTienId`).
    /// - **Xóa (LoaiYeuCauId = 3)**: Hủy đăng ký xe hiện có (yêu cầu `PhuongTienId`).
    /// </remarks>
    [HttpPost("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauPhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TaoYeuCauPhuongTien(
        [FromBody] TaoYeuCauPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cập nhật yêu cầu về phương tiện - Dành cho cư dân
    /// </summary>
    /// <remarks>
    /// API cho phép cư dân cập nhật, gửi (Submit) hoặc thu hồi (Withdraw) yêu cầu phương tiện:
    /// - **Cập nhật**: Chỉnh sửa thông tin khi yêu cầu đang ở trạng thái `Saved`.
    /// - **Gửi (`IsSubmit = true`)**: Chuyển trạng thái yêu cầu từ `Saved` sang `Pending`.
    /// - **Thu hồi (`IsWithdraw = true`)**: Chuyển trạng thái yêu cầu từ `Pending/Saved` sang `Withdrawn`.
    /// </remarks>
    [HttpPut("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauPhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapNhatYeuCauPhuongTien(
        [FromBody] CapNhatYeuCauPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
