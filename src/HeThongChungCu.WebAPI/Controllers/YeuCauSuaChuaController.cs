using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TiepNhanYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.ChotUuTienYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetListYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
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
[Route("api/yeu-cau-sua-chua")]
public class YeuCauSuaChuaController : ApiControllerBase
{
    private readonly ISender _sender;

    public YeuCauSuaChuaController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách yêu cầu sửa chữa
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản lý và cư dân xem danh sách các yêu cầu sửa chữa đã gửi.
    /// - **Hệ thống xử lý**: Lọc theo căn hộ, trạng thái, loại sự cố và khoảng thời gian.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<YeuCauSuaChuaResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromBody] GetListYeuCauSuaChuaQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết yêu cầu sửa chữa
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem chi tiết thông tin, file đính kèm và nhân sự thực hiện của một yêu cầu.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromBody] GetYeuCauSuaChuaByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Cư dân gửi yêu cầu sửa chữa mới
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi cư dân phát hiện sự cố (điện, nước, nội thất...) và muốn yêu cầu BQL hỗ trợ sửa chữa.
    /// - **Hệ thống xử lý**: Khởi tạo yêu cầu với trạng thái mặc định là "Approved" (tự động duyệt) và "MoiTao".
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `CanHoId`, `PhamViId`, `LoaiSuCoId`, `MucDoUuTienDeXuatId`.
    ///     - **Tùy chọn**: `NoiDung`, `MoTaViTri`, `DanhSachTepIds`.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// BQL xác nhận tiếp nhận yêu cầu
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL hoặc tổ kỹ thuật xác nhận đã thấy yêu cầu và bắt đầu xử lý/điều phối.
    /// - **Hệ thống xử lý**: Chuyển trạng thái sang "DaTiepNhan", ghi nhận người thụ lý và ngày giờ tiếp nhận.
    /// </remarks>
    [HttpPost("tiep-nhan")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TiepNhan([FromBody] TiepNhanYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// BQL chốt mức độ ưu tiên
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL sau khi xem xét mức độ nghiêm trọng sẽ chốt lại mức độ ưu tiên thực tế (có thể khác với đề xuất của cư dân).
    /// - **Hệ thống xử lý**: Cập nhật `MucDoUuTienChotId` và ghi nhận người chốt.
    /// </remarks>
    [HttpPost("chot-uu-tien")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChotUuTien([FromBody] ChotUuTienYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
