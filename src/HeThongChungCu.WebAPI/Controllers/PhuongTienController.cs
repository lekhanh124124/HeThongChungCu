using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.PhuongTien.Commands.CapNhatThongTinPhuongTien;
using HeThongChungCu.Application.Features.PhuongTien.Commands.CapNhatTrangThaiPhuongTien;
using HeThongChungCu.Application.Features.PhuongTien.Commands.DangKyPhuongTien;
using HeThongChungCu.Application.Features.PhuongTien.Commands.KhoaThePhuongTien;
using HeThongChungCu.Application.Features.PhuongTien.Commands.DeletePhuongTien;
using HeThongChungCu.Application.Features.PhuongTien.Commands.TaoThePhuongTien;
using HeThongChungCu.Application.Features.PhuongTien.DTOs;
using HeThongChungCu.Application.Features.PhuongTien.Queries.GetPhuongTienById;
using HeThongChungCu.Application.Features.PhuongTien.Queries.LayDSPhuongTienTrongChungCu;
using HeThongChungCu.Domain.Enums;
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
    /// Đăng ký phương tiện mới cho căn hộ
    /// </summary>
    /// <remarks>
    /// API dùng để tạo một bản ghi phương tiện mới thuộc về một `CanHo`.
    /// Yêu cầu cung cấp `CanHoId`, `TenPhuongTien`, ID loại phương tiện (`LoaiPhuongTienId`), `BienSo` và `MauXe`.
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
    /// API dùng để sửa thông tin cơ bản của phương tiện như tên, loại, biển số, màu xe.
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
    /// Cập nhật trạng thái cho danh sách phương tiện
    /// </summary>
    /// <remarks>
    /// API dùng để duyệt hoặc từ chối một hoặc nhiều phương tiện bằng cách truyền vào danh sách `PhuongTienIds` và `TrangThaiPhuongTienId`.
    /// </remarks>
    [HttpPut("trang-thai")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapNhatTrangThaiPhuongTien(
        [FromBody] CapNhatTrangThaiPhuongTienCommand command,
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
    [HttpPost("tao-the")]
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
    /// Khóa thẻ phương tiện (cho phép khóa nhiều thẻ một lúc)
    /// </summary>
    [HttpPut("khoa-the")]
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
    /// Xóa phương tiện (Xóa mềm)
    /// </summary>
    /// <param name="command">Danh sách ID phương tiện cần xóa</param>
    /// <param name="cancellationToken">Token hủy bỏ tác vụ</param>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromBody] DeletePhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
