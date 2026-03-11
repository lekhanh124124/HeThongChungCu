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
    /// Yêu cầu truyền vào `QuanHeCuTruId` và `NgayKetThuc`. Hệ thống sẽ cập nhật trạng thái `IsKetThuc` thành true.
    /// </remarks>
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
    /// <remarks>
    /// API dùng để sửa thông tin lịch sử cư trú hiện tại.
    /// Cho phép đổi loại quan hệ (`LoaiQuanHeCuTruId`) hoặc chỉnh sửa `NgayBatDau`, `NgayKetThuc` của bản ghi `QuanHeCuTru` đó.
    /// </remarks>
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
    /// <remarks>
    /// API truy vấn danh sách những cư dân hiện tại (chưa kết thúc cư trú) thuộc về một `CanHo` cụ thể.
    /// </remarks>
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
    /// <remarks>
    /// Dùng để tra cứu lịch sử cư trú. 
    /// - Nếu truyền `CanHoId`: Trả về lịch sử tất cả cư dân (kể cả đã chuyển đi) của căn hộ đó.
    /// - Nếu truyền `UserId`: Trả về lịch sử các căn hộ mà người đó đã/đang ở.
    /// Hỗ trợ tìm kiếm, lọc trạng thái, phân trang và sắp xếp.
    /// </remarks>
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
    /// <remarks>
    /// Dùng khi cần tra cứu nhanh người dùng trong hệ thống (nhằm thêm cư dân vào căn hộ).
    /// Chỉ trả về những user có quyền Resident/Guest, hỗ trợ tìm kiếm theo Username.
    /// </remarks>
    [HttpPost("search-user")]
    [ProducesResponseType(typeof(ApiResponse<SearchUserByUsernameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TimKiemNguoiDung([FromBody] GetUserByUsernameQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
