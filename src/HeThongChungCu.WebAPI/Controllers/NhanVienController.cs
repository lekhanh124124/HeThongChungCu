using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLNhanVien.Commands.CreateNhanVien;
using HeThongChungCu.Application.Features.QLNhanVien.Commands.DeleteNhanVien;
using HeThongChungCu.Application.Features.QLNhanVien.Commands.UpdateNhanVien;
using HeThongChungCu.Application.Features.QLNhanVien.DTOs;
using HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienList;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/nhan-vien")]
public class NhanVienController : ApiControllerBase
{
    private readonly ISender _sender;

    public NhanVienController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tạo mới một nhân viên tòa nhà
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản trị viên thêm mới nhân viên (kỹ thuật, bảo vệ, vệ sinh) vào hệ thống.
    /// - **Hệ thống xử lý**: 
    ///     - **Tạo Hồ sơ Người dùng**: Tự động tạo thông tin nhân thân (Họ tên, ngày sinh, CCCD...).
    ///     - **Tạo Tài khoản**: Tự động tạo tài khoản đăng nhập với Email và Password cung cấp, gán quyền `Staff`.
    ///     - **Tạo mã nhân viên**: Tự động tạo mã nhân viên theo định dạng `NV-{YYYY}-{XXXX}` (ví dụ: NV-2023-0001).
    ///     - **Khởi tạo nhân viên**: Gán loại nhân viên và trạng thái mặc định "Đang làm việc".
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Thông tin cá nhân**: `Ho`, `Ten`, `NgaySinh`, `GioiTinhId`, `CCCD`, `SoDienThoai`.
    ///     - **Tài khoản**: `Email`, `Password`.
    ///     - **Nhân viên**: `LoaiNhanVienId`, `NgayVaoLam`, `AnhDaiDienId`.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NhanVienDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateNhanVienCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin nhân viên
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cập nhật chức vụ, trạng thái làm việc hoặc thông tin nhân thân của nhân viên.
    /// - **Hệ thống xử lý**: 
    ///     - Cập nhật thông tin hồ sơ nhân viên.
    ///     - Cập nhật thông tin người dùng được liên kết (Họ tên, địa chỉ, CCCD...).
    ///     - Tự động ghi nhận `NgayNghiLam` nếu trạng thái chuyển sang "Đã nghỉ việc".
    /// - **Lưu ý**: Mã nhân viên (`MaNhanVien`) là không thể thay đổi sau khi tạo.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`, `Ho`, `Ten`, `NgaySinh`, `GioiTinhId`, `LoaiNhanVienId`, `TrangThaiNhanVienId`, `NgayVaoLam`.
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<NhanVienDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateNhanVienCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa một hoặc nhiều nhân viên (Xóa mềm)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Loại bỏ danh sách nhân viên khỏi hệ thống quản lý.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra sự tồn tại của tất cả các ID cung cấp.
    ///     - Đánh dấu nhân viên là đã xóa.
    ///     - Chuyển trạng thái nhân viên sang "Đã nghỉ việc".
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Ids` (Danh sách ID nhân viên).
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<int>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteNhanVienCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách nhân viên (hỗ trợ tìm kiếm, lọc, phân trang)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản lý và tìm kiếm nhân viên theo loại hình (kỹ thuật/vệ sinh) hoặc trạng thái làm việc.
    /// - **Hệ thống xử lý**: 
    ///     - Tìm kiếm theo Mã nhân viên, Họ tên hoặc Số điện thoại qua `Keyword`.
    ///     - Kết hợp dữ liệu từ bảng người dùng để lấy thông tin liên lạc.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<NhanVienResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetNhanVienListQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết nhân viên theo ID
    /// </summary>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<NhanVienDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetNhanVienByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
