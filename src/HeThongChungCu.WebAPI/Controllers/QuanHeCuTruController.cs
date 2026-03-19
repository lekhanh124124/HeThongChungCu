using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.Commands.CapNhatQuanHe;
using HeThongChungCu.Application.Features.QuanHeCuTru.Commands.KetThucCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.Commands.ThietLapCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayDSCuDanTrongChungCu;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByPhoneNumber;
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
    /// <remarks>
    /// API dùng để tạo một liên kết cư trú giữa một `User` và một `CanHo`.
    /// Yêu cầu cung cấp `UserId`, `CanHoId`, ID biểu thị loại quan hệ (`LoaiQuanHeCuTruId`) và Ngày bắt đầu (`NgayBatDau`).
    /// </remarks>
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
    /// <remarks>
    /// API dùng để kết thúc khoảng thời gian sinh sống của cư dân tại căn hộ.
    /// Yêu cầu truyền vào `QuanHeCuTruId`. Hệ thống sẽ cập nhật trạng thái `TrangThaiCuTruId` thành `DaKetThuc` và cập nhật 'NgayKetThuc' của quan hệ cư trú.
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> KetThucCuTru([FromBody] KetThucCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật quan hệ cư trú (loại quan hệ trong căn hộ)
    /// </summary>
    /// <remarks>
    /// API dùng để sửa thông tin về quan hệ cư trú
    /// Cho phép đổi loại quan hệ (`LoaiQuanHeCuTruId`).
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapNhatQuanHe([FromBody] CapNhatQuanHeCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách cư dân trong chung cư với bộ lọc và tìm kiếm nâng cao
    /// </summary>
    /// <remarks>
    /// API truy vấn danh sách cư dân hỗ trợ các chức năng:
    /// - **Phạm vi (ID)**: Lọc chính xác theo ToaNhaId, TangId, CanHoId.
    /// - **Từ khóa**: Tìm kiếm theo HoTen, MaToaNha, MaTang, MaCanHo (qua tham số Keyword).
    /// - **Bộ lọc**: 
    ///     - Mã định danh: MaToaNha, MaTang, MaCanHo.
    ///     - Trạng thái: LoaiQuanHeCuTruId, TrangThaiCuTruId.
    ///     - Thời gian: Khoảng ngày bắt đầu (NgayBatDauFrom/To) và khoảng ngày kết thúc (NgayKetThucFrom/To).
    /// - **Sắp xếp**: Hỗ trợ sắp xếp theo MaToaNha, MaTang, MaCanHo, HoTen, LoaiQuanHeCuTruId, NgayBatDau, NgayKetThuc, TrangThaiCuTruId.
    /// - **Phân trang**: PageNumber và PageSize.
    /// </remarks>
    [HttpPost("cu-dan")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CuDanResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LayCuDan([FromBody] LayDSCuDanTrongChungCuQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy lịch sử cư trú của một cư dân
    /// </summary>
    /// <remarks>
    /// API truy vấn lịch sử các lần cư trú của một người dựa trên UserId:
    /// - **Người dùng**: Lọc theo UserId (Bắt buộc).
    /// - **Thông tin cư trú**: Loại quan hệ cư trú.
    /// - **Thời gian**: Khoảng ngày bắt đầu (NgayBatDauFrom/To) và khoảng ngày kết thúc (NgayKetThucFrom/To).
    /// - **Sắp xếp**: Hỗ trợ sắp xếp theo NgayBatDau, NgayKetThuc, MaCanHo, LoaiQuanHeCuTruId.
    /// - **Phân trang**: PageNumber và PageSize.
    /// </remarks>
    [HttpPost("lich-su")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<LichSuCuTruResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LayLichSuCuTru([FromBody] LayLichSuCuTruQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Tìm kiếm người dùng theo số điện thoại (chỉ Resident hoặc Guest)
    /// </summary>
    /// <remarks>
    /// Dùng khi cần tra cứu nhanh người dùng trong hệ thống (nhằm thêm cư dân vào căn hộ).
    /// Chỉ trả về những user có quyền Resident/Guest, hỗ trợ tìm kiếm theo PhoneNumber.
    /// </remarks>
    [HttpPost("search-user")]
    [ProducesResponseType(typeof(ApiResponse<SearchUserByUsernameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TimKiemNguoiDung([FromBody] GetUserByPhoneNumberQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
