using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.Commands.KetThucCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Commands.ThietLapCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Commands.PheDuyetYeuCauCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Commands.TuChoiYeuCauCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSCuDanTrongChungCu;
using HeThongChungCu.Application.Features.QLCuTru.Commands.TaoHoSo;
using HeThongChungCu.Application.Features.QLCuTru.Commands.TaoMaDinhDanh;
using HeThongChungCu.Application.Features.QLCuTru.Queries.TimHoSoTheoCCCD;
using HeThongChungCu.Application.Features.QLCuTru.Commands.DinhDanhNguoiDung;
using HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatYeuCauCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Commands.ChinhSuaHoSo;
using HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;
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

    #region Group 1: Quản lý hồ sơ cư trú

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
    /// Tìm hồ sơ cư dân (Search User) theo CCCD
    /// </summary>
    /// <remarks>
    /// API sử dụng khi Nhân viên BQL muốn kiểm tra xem cư dân đã có hồ sơ trong hệ thống hay chưa dựa trên thông tin CCCD.
    /// Tra cứu thông tin để tránh tạo trùng lặp hồ sơ người dùng và chuẩn bị cho bước thiết lập cư trú.
    /// </remarks>
    [HttpPost("search-user")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TimHoSoTheoCCCD([FromBody] TimHoSoTheoCCCDQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Thiết lập cư trú – thêm cư dân vào căn hộ (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// API dành cho BQL để gán một Hồ sơ người dùng vào một căn hộ cụ thể.
    /// Hệ thống sẽ tạo một bản ghi Quan hệ cư trú (QuanHeCuTru) liên kết User với CanHo.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CuDanResponse>), StatusCodes.Status200OK)]
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
    /// Tạo hồ sơ cư dân mới
    /// </summary>
    /// <remarks>
    /// API dùng để tạo mới một Hồ sơ người dùng (User) khi cư dân đó chưa từng tồn tại trong hệ thống.
    /// Hồ sơ này chỉ chứa thông tin định danh cơ bản, chưa liên kết với căn hộ hay tài khoản đăng nhập.
    /// </remarks>
    [HttpPost("ho-so")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TaoHoSo([FromBody] TaoHoSoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Chỉnh sửa hồ sơ cư dân (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// API dùng để chỉnh sửa thông tin hồ sơ của cư dân trong căn hộ.
    /// Cho phép cập nhật thông tin cá nhân và các tài liệu cư trú đi kèm.
    /// </remarks>
    [HttpPut("ho-so")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChinhSuaHoSo([FromBody] ChinhSuaHoSoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    #endregion

    #region Group 2: Quản lý yêu cầu cư trú

    /// <summary>
    /// Lấy danh sách yêu cầu cư trú (Dành cho BQL) với bộ lọc, sắp xếp và phân trang nâng cao
    /// </summary>
    /// <remarks>
    /// API truy vấn danh sách các yêu cầu cư trú hỗ trợ các chức năng:
    /// - **Phạm vi**: Lọc theo căn hộ (`CanHoId`).
    /// - **Loại yêu cầu**: Lọc theo loại yêu cầu (`LoaiYeuCauId` - Thêm, Sửa, Xóa).
    /// - **Trạng thái**: Lọc theo trạng thái xử lý (`TrangThaiId` - Chờ duyệt, Đã duyệt, Từ chối).
    /// - **Sắp xếp**: Hỗ trợ sắp xếp theo các cột thông qua `SortCol` và `IsAsc`.
    /// - **Phân trang**: Hỗ trợ phân trang qua `PageNumber` và `PageSize`.
    /// </remarks>
    [HttpPost("yeu-cau/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<YeuCauCuTruResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LayDSYeuCau([FromBody] LayDSYeuCauCuTruQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết yêu cầu cư trú
    /// </summary>
    [HttpPost("yeu-cau/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauCuTruResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYeuCauById([FromBody] GetYeuCauCuTruByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Tạo yêu cầu cư trú (Dành cho Cư dân)
    /// </summary>
    /// <remarks>
    /// API cho phép Chủ hộ chủ động gửi yêu cầu Thêm/Sửa/Xóa thành viên cư trú trong căn hộ của mình.
    /// Yêu cầu sẽ được lưu dưới dạng bản ghi tạm thời, bao gồm cả thông tin thành viên đề xuất và các tài liệu đính kèm, chờ BQL phê duyệt.
    /// </remarks>
    [HttpPost("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauCuTruResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TaoYeuCau([FromBody] TaoYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật yêu cầu cư trú (Dành cho Cư dân)
    /// </summary>
    /// <remarks>
    /// API cho phép cư dân chỉnh sửa yêu cầu đang ở trạng thái "Đã lưu".
    /// Có thể chuyển sang trạng thái "Đang chờ duyệt" (IsSubmit = true) hoặc "Đã thu hồi" (IsWithdraw = true).
    /// </remarks>
    [HttpPut("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauCuTruResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CapNhatYeuCau([FromBody] CapNhatYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa yêu cầu cư trú (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// API cho phép BQL xóa các yêu cầu dư thừa hoặc không cần thiết. Cho phép xóa nhiều yêu cầu cùng lúc.
    /// </remarks>
    [HttpDelete("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> XoaYeuCau([FromBody] XoaYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Phê duyệt yêu cầu cư trú (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// API dùng để BQL duyệt yêu cầu do cư dân gửi lên. 
    /// Sau khi duyệt:
    /// - Nếu là yêu cầu Thêm: Hệ thống tự động tạo User, thêm tài liệu và gán vào căn hộ.
    /// - Nếu là yêu cầu Sửa/Xóa: Cập nhật thông tin thực tế của cư dân tương ứng.
    /// </remarks>
    [HttpPost("yeu-cau/phe-duyet")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PheDuyetYeuCau([FromBody] PheDuyetYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Từ chối yêu cầu cư trú (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// BQL dùng API này để bác bỏ yêu cầu của cư dân nếu thông tin không hợp lệ hoặc thiếu tài liệu.
    /// Yêu cầu bắt buộc phải truyền vào Lý do từ chối để thông báo cho cư dân.
    /// </remarks>
    [HttpPost("yeu-cau/tu-choi")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TuChoiYeuCau([FromBody] TuChoiYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    #endregion

    #region Group 4: Liên kết tài khoản

    /// <summary>
    /// Tạo mã định danh (sinh token)
    /// </summary>
    /// <remarks>
    /// API dùng để sinh ra một mã xác thực (Token) để cư dân có thể sử dụng mã này tự liên kết với tài khoản ứng dụng cá nhân.
    /// Giúp bảo mật quá trình bàn giao tài khoản cho cư dân chính chủ.
    /// </remarks>
    [HttpPost("tao-ma-dinh-danh")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TaoMaDinhDanh([FromBody] TaoMaDinhDanhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xác nhận định danh (dành cho Cư dân - qua link email)
    /// </summary>
    /// <remarks>
    /// API được gọi khi cư dân nhấn vào link xác nhận trong email. 
    /// Hệ thống sẽ giải mã token để lấy thông tin UserId và thực hiện liên kết tài khoản.
    /// </remarks>
    [HttpPost("xac-nhan-dinh-danh")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> XacNhanDinhDanh([FromBody] XacNhanDinhDanhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Liên kết tài khoản trực tiếp (dành cho BQL)
    /// </summary>
    /// <remarks>
    /// API cho phép BQL trực tiếp liên kết một Hồ sơ cư dân với một Tài khoản người dùng dựa trên Email.
    /// Không cần thông qua quy trình gửi email xác nhận.
    /// </remarks>
    [HttpPost("lien-ket-tai-khoan")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LienKetTaiKhoan([FromBody] LienKetTaiKhoanCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    #endregion
}
