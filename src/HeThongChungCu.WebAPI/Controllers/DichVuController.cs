using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.Commands.CreateDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Commands.UpdateDichVu;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Commands.CreateKhungGioDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetListKhungGioDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetKhungGioDichVuById;
using HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeKhungGioDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Commands.CreateBangGia;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetListBangGia;
using HeThongChungCu.Application.Features.QLDichVu.Queries.GetBangGiaById;
using HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeBangGia;
using HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateBangGia;
using HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteBangGia;
using HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateKhungGioDichVu;
using HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteKhungGioDichVu;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/dich-vu")]
public class DichVuController : ApiControllerBase
{
    private readonly ISender _sender;

    public DichVuController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tạo mới một dịch vụ
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi BQL muốn thêm một loại dịch vụ mới (Điện, nước, gửi xe, vệ sinh...) vào hệ thống.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra tính duy nhất của mã dịch vụ (`MaDichVu`).
    ///     - Gán dịch vụ vào loại dịch vụ tương ứng.
    ///     - Có thể liên kết trực tiếp với một hợp đồng đối tác cụ thể.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `MaDichVu`, `TenDichVu`, `LoaiDichVuId`, `DonViTinh`.
    ///     - **Tùy chọn**: `MoTa`, `IconId`, `HopDongDoiTacId`, `IsBatBuoc`.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DichVuResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin dịch vụ
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Điều chỉnh tên dịch vụ, đơn vị tính, mô tả hoặc thay đổi đối tác cung cấp.
    /// - **Hệ thống xử lý**: Cập nhật thông tin chi tiết của dịch vụ vào cơ sở dữ liệu.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`, `TenDichVu`, `LoaiDichVuId`, `DonViTinh`.
    ///     - **Tùy chọn**: `MoTa`, `IconId`, `HopDongDoiTacId`, `IsBatBuoc`.
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<DichVuResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách dịch vụ (hỗ trợ phân trang, lọc theo loại, đối tác, hợp đồng)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản lý tổng thể các dịch vụ trong hệ thống, tra cứu giá và đối tác cung cấp.
    /// - **Hệ thống xử lý**: 
    ///     - Lọc theo `LoaiDichVuId`, `DoiTacId`, `HopDongDoiTacId`.
    ///     - Tìm kiếm theo `Keyword` (Mã hoặc tên dịch vụ).
    ///     - Hỗ trợ phân trang mặc định (10 bản ghi/trang).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Tùy chọn**: `LoaiDichVuId`, `DoiTacId`, `HopDongDoiTacId`, `Keyword`, `PageNumber`, `PageSize`.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DichVuResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetListDichVuQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết dịch vụ theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem cấu hình chi tiết của một dịch vụ, bao gồm cả bảng giá hiện tại.
    /// - **Hệ thống xử lý**: Truy xuất bản ghi dịch vụ và các thông tin liên quan (đối tác, bảng giá).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<DichVuDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] GetDichVuByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Kích hoạt dịch vụ
    /// </summary>
    [HttpPut("activate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Activate([FromBody] ActivateDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Thu hồi dịch vụ (Ngừng cung cấp)
    /// </summary>
    [HttpPut("revoke")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Revoke([FromBody] RevokeDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa dịch vụ
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromBody] DeleteDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Tạo mới một khung giờ dịch vụ
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Thiết lập khung giờ cho các dịch vụ đăng ký theo giờ (gym, hồ bơi, phòng họp...).
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra sự tồn tại của dịch vụ.
    ///     - Kiểm tra trùng lặp (overlap) với các khung giờ đã có của dịch vụ đó.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `DichVuId`, `GioBatDau`, `GioKetThuc`, `TenKhungGio`.
    ///     - **Tùy chọn**: `NgayTrongTuan` (0-6, null là mọi ngày).
    /// </remarks>
    [HttpPost("khung-gio")]
    [ProducesResponseType(typeof(ApiResponse<KhungGioDichVuResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateKhungGio([FromBody] CreateKhungGioDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Thu hồi các khung giờ dịch vụ (Bulk Revoke)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi một hoặc nhiều khung giờ không còn hoạt động. 
    /// - **Hệ thống xử lý**: Chuyển trạng thái `IsActive` thành `false`.
    /// </remarks>
    [HttpDelete("khung-gio/revoke")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeKhungGio([FromBody] RevokeKhungGioDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult<bool>(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách khung giờ dịch vụ
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem danh sách các khung giờ của một dịch vụ cụ thể hoặc tìm kiếm theo tên.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Tùy chọn**: `DichVuId`, `Keyword`, `PageNumber`, `PageSize`.
    /// </remarks>
    [HttpPost("khung-gio/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<KhungGioDichVuResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetKhungGioList([FromBody] GetListKhungGioDichVuQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết khung giờ dịch vụ theo ID
    /// </summary>
    [HttpPost("khung-gio/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<KhungGioDichVuResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetKhungGioById([FromBody] GetKhungGioDichVuByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Kích hoạt khung giờ dịch vụ
    /// </summary>
    [HttpPut("khung-gio/activate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateKhungGio([FromBody] ActivateKhungGioDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa khung giờ dịch vụ
    /// </summary>
    [HttpDelete("khung-gio")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteKhungGio([FromBody] DeleteKhungGioDichVuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Tạo mới một bảng giá dịch vụ
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Thiết lập đơn giá mới cho dịch vụ (Có thể là giá cố định, lũy tiến hoặc theo khung giờ).
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra sự tồn tại của dịch vụ.
    ///     - Kiểm tra trùng lấn (overlap) ngày áp dụng với các bảng giá hiện có của dịch vụ đó.
    ///     - Khởi tạo các chi tiết giá mẫu (bậc thang hoặc giá theo giờ) nếu có.
    /// </remarks>
    [HttpPost("bang-gia")]
    [ProducesResponseType(typeof(ApiResponse<BangGiaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBangGia([FromBody] CreateBangGiaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult<BangGiaResponse>(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Thu hồi các bảng giá dịch vụ (Bulk Revoke)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi một hoặc nhiều bảng giá bị sai hoặc không còn áp dụng. 
    /// - **Hệ thống xử lý**: Chuyển trạng thái `IsActive` thành `false` và gán ngày kết thúc nếu đang null.
    /// </remarks>
    [HttpDelete("bang-gia/revoke")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeBangGia([FromBody] RevokeBangGiaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult<bool>(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách bảng giá dịch vụ
    /// </summary>
    [HttpPost("bang-gia/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BangGiaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBangGiaList([FromBody] GetListBangGiaQuery query, CancellationToken cancellationToken)
    {
        return HandleResult<PagedResult<BangGiaResponse>>(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết bảng giá dịch vụ theo ID
    /// </summary>
    [HttpPost("bang-gia/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<BangGiaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBangGiaById([FromBody] GetBangGiaByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult<BangGiaResponse?>(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Kích hoạt bảng giá dịch vụ
    /// </summary>
    [HttpPut("bang-gia/activate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateBangGia([FromBody] ActivateBangGiaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa bảng giá dịch vụ
    /// </summary>
    [HttpDelete("bang-gia")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBangGia([FromBody] DeleteBangGiaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
