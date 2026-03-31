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
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL truy vấn danh sách toàn bộ phương tiện trong chung cư để kiểm soát số lượng, vị trí đỗ và trạng thái vận hành.
    /// - **Hệ thống xử lý**: 
    ///     - Truy vấn kết hợp thông tin phương tiện, chủ sở hữu (căn hộ) và vị trí (tòa nhà/tầng).
    ///     - Hỗ trợ bộ lọc động theo loại xe, biển số, màu sắc và trạng thái (tìm kiếm theo tên xe, biển số, màu xe qua Keyword).
    ///     - Thực hiện phân trang và sắp xếp phía Server.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PageNumber`, `PageSize`.
    ///     - **Tùy chọn (Filter)**: `ToaNhaId`, `TangId`, `CanHoId`, `Keyword`, `LoaiPhuongTienId` (api/catalog/loai-phuong-tien-for-selector), `MauXe`, `TrangThaiPhuongTienId` (api/catalog/trang-thai-phuong-tien-for-selector), `SortCol`, `IsAsc`.
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
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem chi tiết thông số kỹ thuật, hình ảnh và danh sách các thẻ xe (RFID) đang liên kết với phương tiện này.
    /// - **Hệ thống xử lý**: Truy xuất thông tin phương tiện kèm theo các tệp hình ảnh và thông tin thẻ xe liên quan.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`.
    /// </remarks>
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
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL thực hiện đăng ký xe chính thức cho cư dân sau khi kiểm tra hồ sơ giấy tờ trực tiếp.
    /// - **Hệ thống xử lý**: 
    ///     - Xác thực sự tồn tại của căn hộ.
    ///     - Kiểm tra hạn mức (quota) số lượng xe tối đa của căn hộ để đảm bảo không vượt quá quy định.
    ///     - Tạo bản ghi phương tiện và lưu trữ thông tin hình ảnh đi kèm.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `CanHoId`, `TenPhuongTien`, `LoaiPhuongTienId` (Lấy tại api/catalog/loai-phuong-tien-for-selector), `BienSo`, `MauXe`.
    ///     - **Tùy chọn**: `HinhAnhIds`.
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
    /// - **Hoàn cảnh sử dụng**: BQL sửa đổi các thông tin cơ bản của xe (tên xe, biển số, màu sắc) do nhập sai hoặc cư dân thay đổi thông tin.
    /// - **Hệ thống xử lý**: Cập nhật các thuộc tính của bản ghi phương tiện và cập nhật lại danh sách hình ảnh đính kèm.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PhuongTienId`, `TenPhuongTien`, `LoaiPhuongTienId` (api/catalog/loai-phuong-tien-for-selector), `BienSo`, `MauXe`.
    ///     - **Tùy chọn**: `HinhAnhIds`.
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
    /// - **Hoàn cảnh sử dụng**: Khôi phục trạng thái hoạt động cho các phương tiện đang bị khóa hoặc tạm dừng.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra lại hạn mức xe của căn hộ tại thời điểm kích hoạt (để tránh trường hợp đã hết chỗ trong lúc phương tiện đang bị khóa).
    ///     - Cập nhật trạng thái phương tiện sang "Active".
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PhuongTienIds` (Danh sách ID phương tiện).
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
    /// - **Hoàn cảnh sử dụng**: Chấm dứt quyền gửi xe của cư dân (ví dụ: cư dân chuyển đi hoặc không còn nhu cầu gửi xe).
    /// - **Hệ thống xử lý**: 
    ///     - Chuyển trạng thái phương tiện sang "Đã hủy" (Inactive).
    ///     - Tự động khóa toàn bộ các thẻ xe đang liên kết với phương tiện này để ngăn chặn việc ra vào.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PhuongTienIds` (Danh sách ID phương tiện).
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
    /// - **Hoàn cảnh sử dụng**: BQL tạm thời đình chỉ quyền gửi xe của cư dân do vi phạm quy định hoặc nợ phí gửi xe.
    /// - **Hệ thống xử lý**: 
    ///     - Cập nhật trạng thái phương tiện sang "Bị khóa" (Blocked).
    ///     - Vô hiệu hóa tính năng quẹt thẻ của thẻ xe liên kết cho đến khi được mở khóa lại.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PhuongTienIds` (Danh sách ID phương tiện).
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
    /// - **Hoàn cảnh sử dụng**: Gán một thẻ vật lý (chip RFID) cho một phương tiện đã đăng ký để bắt đầu sử dụng dịch vụ trông giữ xe.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra tính duy nhất của mã thẻ (`MaThe`) trong hệ thống.
    ///     - Tạo liên kết giữa thẻ và phương tiện, thiết lập trạng thái hoạt động cho thẻ.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PhuongTienId`, `MaThe`.
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
    /// - **Hoàn cảnh sử dụng**: Hỗ trợ BQL nhanh chóng lấy một mã thẻ gợi ý (thường dựa trên mã phương tiện hoặc số thứ tự) khi cấp thẻ mới.
    /// - **Hệ thống xử lý**: Sinh mã thẻ gợi ý dựa trên quy tắc đánh số của hệ thống và kiểm tra tính khả dụng.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PhuongTienId`.
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
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL vô hiệu hóa thủ công một hoặc nhiều thẻ xe cụ thể (ví dụ: thẻ bị hỏng hoặc thu hồi riêng lẻ).
    /// - **Hệ thống xử lý**: Chuyển trạng thái thẻ sang "Bị khóa", ngăn chặn quẹt thẻ tại các máy iParking.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `TheIds` (Danh sách ID thẻ).
    /// </remarks>
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
    /// - **Hoàn cảnh sử dụng**: Cư dân chủ động báo cáo khi bị mất thẻ gửi xe để đảm bảo an ninh, tránh kẻ gian sử dụng thẻ.
    /// - **Hệ thống xử lý**: 
    ///     - Xác thực quyền sở hữu của cư dân đối với thẻ xe.
    ///     - Ngay lập tức chuyển trạng thái các thẻ trong danh sách sang "Khóa" và ghi nhận lý do bảo mật.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `TheIds` (Danh sách ID thẻ).
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
    /// - **Hoàn cảnh sử dụng**: Cư dân gửi yêu cầu đăng ký mới, sửa đổi thông tin hoặc hủy phương tiện thông qua ứng dụng Mobile.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra quyền truy cập của cư dân vào căn hộ được gửi yêu cầu.
    ///     - Lưu trữ nội dung dưới dạng "Yêu cầu chờ duyệt" (Pending) hoặc "Bản nháp" (Saved).
    ///     - Toàn bộ thay đổi sẽ chỉ có hiệu lực sau khi được BQL phê duyệt chính thức.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `CanHoId`, `LoaiYeuCauId` (api/catalog/loai-yeu-cau-for-selector).
    ///     - **Trường hợp Thêm (LoaiYeuCauId = 1)**: Cần `YeuCauLoaiPhuongTienId` (api/catalog/loai-phuong-tien-for-selector), `YeuCauTenPhuongTien`, `YeuCauBienSo`, `YeuCauMauXe`.
    ///     - **Trường hợp Sửa/Xóa (LoaiYeuCauId = 2, 3)**: Cần `YeuCauPhuongTienId`.
    ///     - **Tùy chọn**: `FileIds`.
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
    /// - **Hoàn cảnh sử dụng**: Cư dân quản lý các yêu cầu phương tiện hiện có (gửi duyệt hoặc thu hồi).
    /// - **Hệ thống xử lý**: 
    ///     - Cho phép chỉnh sửa thông tin yêu cầu khi đang ở trạng thái "Bản nháp" (Saved).
    ///     - Chuyển trạng thái yêu cầu sang "Đang chờ duyệt" (IsSubmit = true) để BQL nhìn thấy hoặc "Đã rút" (IsWithdraw = true) để hủy yêu cầu.
    /// - **Cơ chế nộp và rút yêu cầu**:
    ///     - `IsSubmit = true`: Chốt dữ liệu và gửi cho BQL phê duyệt (chuyển từ "Đã lưu" sang "Chờ duyệt"). Sau khi nộp, cư dân không thể tự chỉnh sửa.
    ///     - `IsWithdraw = true`: Cư dân chủ động rút lại yêu cầu (chuyển sang trạng thái "Đã rút"). Hành động này ưu tiên hơn cập nhật nội dung.
    ///     - Nếu cả hai đều `false`: Chỉ cập nhật thay đổi nội dung và giữ yêu cầu ở trạng thái "Đã lưu".
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`.
    ///     - **Khi cập nhật nội dung (IsSubmit/IsWithdraw = false)**: `YeuCauTenPhuongTien`, `YeuCauBienSo`, `YeuCauMauXe`.
    ///     - **Khi gửi/thu hồi**: Chỉ cần `Id` và `IsSubmit=true` hoặc `IsWithdraw=true`.
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
