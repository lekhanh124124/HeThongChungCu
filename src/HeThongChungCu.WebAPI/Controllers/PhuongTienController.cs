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
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.XoaYeuCauPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.PheDuyetYeuCauPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Commands.TuChoiYeuCauPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GoiYMaThePhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSPhuongTienTrongChungCu;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;
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
    ///     - **Lưu ý về Tệp tin**: Các tệp tin hình ảnh phải được tải lên trước thông qua API `POST api/upload-media` để lấy danh sách `Id`. Sau đó, sử dụng các `Id` này để điền vào trường `HinhAnhIds`.
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
    /// - **Lưu ý về Tệp tin**: Các tệp tin hình ảnh mới phải được tải lên trước thông qua API `POST api/upload-media` để lấy danh sách `Id`. Sau đó, sử dụng các `Id` này để điền vào trường `HinhAnhIds`.
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
    ///     - **Lưu ý về Tệp tin**: Các tệp tin hình ảnh phải được tải lên trước thông qua API `POST api/upload-media` để lấy danh sách `Id`. Sau đó, sử dụng các `Id` này để điền vào trường `FileIds`.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `CanHoId`, `LoaiYeuCauId` (api/catalog/loai-yeu-cau-for-selector), `IsSubmit`.
    ///     - **IsSubmit**:
    ///         - `true`: Chốt dữ liệu và gửi cho BQL phê duyệt (Trạng thái "Chờ duyệt").
    ///         - `false`: Lưu tạm thời để chỉnh sửa sau (Trạng thái "Đã lưu").
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
    ///     - **Lưu ý về Tệp tin**: Các tệp tin hình ảnh mới phải được tải lên trước thông qua API `POST api/upload-media` để lấy danh sách `Id`. Sau đó, sử dụng các `Id` này để điền vào trường `FileIds`.
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

    /// <summary>
    /// Xóa yêu cầu phương tiện (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL dọn dẹp các yêu cầu rác, yêu cầu bị gửi nhầm hoặc không còn giá trị xử lý.
    /// - **Hệ thống xử lý**: Thực hiện xóa cứng các bản ghi yêu cầu tương ứng khỏi hệ thống.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Ids` (Danh sách ID yêu cầu).
    /// </remarks>
    [HttpDelete("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> XoaYeuCau(
        [FromBody] XoaYeuCauPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết yêu cầu phương tiện
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Xem chi tiết nội dung của một yêu cầu (Thêm/Sửa/Xóa xe) kèm theo các hình ảnh minh chứng và thông tin người gửi/xử lý.
    /// - **Hệ thống xử lý**: Truy xuất thông tin yêu cầu kết hợp với thông tin căn hộ, tòa nhà và lịch sử xử lý.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `RequestId`.
    /// </remarks>
    [HttpPost("yeu-cau/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauPhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetYeuCauById(
        [FromBody] GetYeuCauPhuongTienByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy danh sách yêu cầu phương tiện với bộ lọc và phân trang
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL truy vấn danh sách các yêu cầu đang chờ xử lý hoặc lịch sử yêu cầu của cư dân. Cư dân theo dõi trạng thái các yêu cầu của chính mình.
    /// - **Hệ thống xử lý**: 
    ///     - Hỗ trợ bộ lọc theo tòa nhà, tầng, căn hộ, loại yêu cầu và trạng thái. 
    ///     - Trả về thông tin tóm tắt kèm theo metadata về người gửi, người xử lý và vị trí căn hộ.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Tùy chọn (Filter)**: `ToaNhaId`, `TangId`, `CanHoId`, `LoaiYeuCauId`, `TrangThaiId`.
    ///     - **Phân trang**: `PageNumber`, `PageSize`.
    /// </remarks>
    [HttpPost("yeu-cau/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DSYeuCauPhuongTienResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYeuCauList(
        [FromBody] LayDSYeuCauPhuongTienQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Phê duyệt yêu cầu về phương tiện (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL phê duyệt yêu cầu đăng ký/thay đổi/hủy xe của cư dân để cập nhật dữ liệu chính thức vào hệ thống.
    /// - **Hệ thống xử lý**: 
    ///     - **Thêm mới**: Tự động tạo bản ghi phương tiện chính thức và lưu trữ hình ảnh.
    ///     - **Chỉnh sửa**: Cập nhật thông tin phương tiện hiện có.
    ///     - **Xóa**: Chấm dứt quyền sử dụng dịch vụ của phương tiện (Inactive).
    ///     - Chuyển trạng thái yêu cầu sang "Đã duyệt" và ghi nhận người xử lý.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `YeuCauPhuongTienId`.
    /// </remarks>
    [HttpPost("yeu-cau/phe-duyet")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauPhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PheDuyetYeuCau(
        [FromBody] PheDuyetYeuCauPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Từ chối yêu cầu về phương tiện (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL không chấp nhận yêu cầu của cư dân (thiếu hình ảnh, thông tin sai, v.v.).
    /// - **Hệ thống xử lý**: Ghi nhận lý do từ chối, chuyển trạng thái yêu cầu sang "Từ chối" và gửi phản hồi đến cư dân qua hệ thống thông báo.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `YeuCauPhuongTienId`, `LyDo`.
    /// </remarks>
    [HttpPost("yeu-cau/tu-choi")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauPhuongTienResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TuChoiYeuCau(
        [FromBody] TuChoiYeuCauPhuongTienCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
