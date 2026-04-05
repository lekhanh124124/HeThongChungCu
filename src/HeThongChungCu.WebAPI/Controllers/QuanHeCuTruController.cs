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
using HeThongChungCu.Application.Features.QLCuTru.Commands.LienKetTaiKhoan;
using HeThongChungCu.Application.Features.QLCuTru.Commands.XacNhanDinhDanh;
using HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatYeuCauCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Commands.ChinhSuaHoSo;
using HeThongChungCu.Application.Features.QLCuTru.Commands.XoaYeuCauCuTru;
using HeThongChungCu.WebAPI.Common.Models;
using HeThongChungCu.WebAPI.Common.Templates;

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
    /// - **Hoàn cảnh sử dụng**: BQL truy vấn danh sách toàn bộ cư dân trong hệ thống để quản lý hồ sơ, phục vụ công tác thống kê hoặc hỗ trợ cư dân.
    /// - **Hệ thống xử lý**: 
    ///     - Truy vấn kết hợp thông tin cư dân, căn hộ và tòa nhà.
    ///     - Hỗ trợ đa dạng các bộ lọc (HoTen, MaToaNha, MaTang, MaCanHo qua Keyword).
    ///     - Sắp xếp và phân trang phía Server.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PageNumber`, `PageSize`.
    ///     - **Tùy chọn (Filter)**: `ToaNhaId`, `TangId`, `CanHoId`, `Keyword`, `MaToaNha`, `MaTang`, `MaCanHo`, `LoaiQuanHeCuTruId` (api/catalog/loai-quan-he-cu-tru-for-selector), `TrangThaiCuTruId` (api/catalog/trang-thai-cu-tru-for-selector), `NgayBatDauFrom/To`, `NgayKetThucFrom/To`, `SortCol`, `IsAsc`.
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
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL kiểm tra sự tồn tại của hồ sơ dựa trên CCCD để quyết định tạo mới hay sử dụng lại hồ sơ cũ khi thiết lập cư trú.
    /// - **Hệ thống xử lý**: Tìm kiếm chính xác hồ sơ cư dân theo mã CCCD trong toàn hệ thống.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `CCCD`.
    /// </remarks>
    [HttpPost("search-user")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TimHoSoTheoCCCD([FromBody] TimHoSoTheoCCCDQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Thiết lập cư trú – thêm cư dân vào căn hộ (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL thực hiện thêm cư dân vào căn hộ một cách thủ công (ví dụ: khi cư dân đến làm việc trực tiếp tại văn phòng).
    /// - **Hệ thống xử lý**: 
    ///     - Xác thực sự tồn tại của căn hộ và hồ sơ người dùng.
    ///     - Kiểm tra điều kiện ràng buộc: Căn hộ phải có ít nhất một "Chủ hộ" đang cư trú trước khi thêm các thành viên khác.
    ///     - Thiết lập quan hệ cư trú và tự động nâng cấp vai trò tài khoản từ "Khách" lên "Cư dân" nếu tài khoản đã tồn tại.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `CanHoId`, `UserId`, `LoaiQuanHeCuTruId` (1-Chủ hộ, 2-Thành viên, 3-Khách thuê, ... - Lấy danh sách tại api/catalog/loai-quan-he-cu-tru-for-selector).
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
    /// - **Hoàn cảnh sử dụng**: Khi cư dân chuyển đi hoặc không còn sinh sống tại căn hộ.
    /// - **Hệ thống xử lý**: 
    ///     - Cập nhật ngày kết thúc cư trú và chuyển trạng thái sang "Đã kết thúc".
    ///     - Tự động thu hồi các quyền truy cập hoặc các dịch vụ liên quan gắn liền với quan hệ cư trú này (ví dụ: thẻ xe - nếu có cấu hình).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `QuanHeCuTruId`.
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
    /// - **Hoàn cảnh sử dụng**: BQL tạo bản ghi hồ sơ cá nhân cho một cư dân hoàn toàn mới trong hệ thống.
    /// - **Hệ thống xử lý**: 
    ///     - Tạo mới một bản ghi Người dùng với thông tin định danh và tài liệu đi kèm.
    ///     - Đảm bảo tính duy nhất của mã số định danh (nếu có cung cấp).
    ///     - **Lưu ý về Tệp tin**: Các tệp tin tài liệu định danh phải được tải lên trước thông qua API `POST api/upload-media` để lấy danh sách `Id`. Sau đó, sử dụng các `Id` này để điền vào trường `FileIds` trong danh sách `TaiLieuCuTrus`.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `FirstName`, `LastName`, `Dob`, `GioiTinhId` (Lấy tại api/catalog/gioi-tinh-for-selector).
    ///     - **Tùy chọn**: `DiaChi`, `IdCard`, `PhoneNumber`, `TaiLieuCuTrus`.
    /// </remarks>
    [HttpPost("ho-so")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TaoHoSo([FromBody] TaoHoSoCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Chỉnh sửa hồ sơ cư dân (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL hoặc cư dân có thẩm quyền cập nhật các thông tin cá nhân hoặc tài liệu định danh của cư dân đang cư trú.
    /// - **Hệ thống xử lý**: Cập nhật thông tin chi tiết của cư dân và đồng bộ hóa các tài liệu pháp lý liên quan.
    /// - **Lưu ý về Tệp tin**: Các tệp tin tài liệu định danh mới phải được tải lên trước thông qua API `POST api/upload-media` để lấy danh sách `Id`. Sau đó, sử dụng các `Id` này để điền vào trường `FileIds` trong danh sách `TaiLieuCuTrus`.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `QuanHeCuTruId`, `FirstName`, `LastName`, `Dob`, `LoaiQuanHeCuTruId` (api/catalog/loai-quan-he-cu-tru-for-selector).
    ///     - **Tùy chọn**: `GioiTinhId` (api/catalog/gioi-tinh-for-selector), `DiaChi`.
    /// </remarks>
    [HttpPut("ho-so")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
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
    /// - **Hoàn cảnh sử dụng**: BQL theo dõi và quản lý luồng yêu cầu chưa được xử lý từ phía cư dân.
    /// - **Hệ thống xử lý**: Truy xuất danh sách yêu cầu kèm theo thông tin chi tiết về căn hộ và loại thay đổi, hỗ trợ phân trang và lọc theo tòa/tầng/căn hộ hoặc trạng thái.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `PageNumber`, `PageSize`.
    ///     - **Tùy chọn (Filter)**: `ToaNhaId`, `TangId`, `CanHoId`, `LoaiYeuCauId` (api/catalog/loai-yeu-cau-for-selector), `TrangThaiId` (api/catalog/trang-thai-yeu-cau-cu-tru-for-selector), `SortCol`, `IsAsc`.
    /// </remarks>
    [HttpPost("yeu-cau/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DSYeuCauCuTruResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LayDSYeuCau([FromBody] LayDSYeuCauCuTruQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết yêu cầu cư trú theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Nhân viên BQL hoặc cư dân xem chi tiết nội dung, trạng thái và các tài liệu đính kèm của một yêu cầu cư trú cụ thể.
    /// - **Hệ thống xử lý**: Truy xuất thông tin yêu cầu cư trú từ cơ sở dữ liệu, bao gồm các thông tin thay đổi và danh sách hồ sơ tài liệu liên quan.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `RequestId`.
    /// </remarks>
    [HttpPost("yeu-cau/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauCuTruResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetYeuCauById([FromBody] GetYeuCauCuTruByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Tạo yêu cầu cư trú (Dành cho Cư dân)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Chủ hộ chủ động cập nhật nhân sự cho căn hộ của mình thông qua ứng dụng (Mobile/Web).
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra quyền hạn của người gửi (phải là Chủ hộ đang cư trú).
    ///     - Lưu trữ thông tin dưới dạng "Yêu cầu chờ duyệt", không thay đổi dữ liệu cư dân thực tế cho đến khi được BQL phê duyệt.
    ///     - **Cơ chế nộp yêu cầu**:
    ///         - `IsSubmit = true`: Chốt dữ liệu và gửi cho BQL phê duyệt (chuyển trạng thái sang "Chờ duyệt"). Sau khi nộp, cư dân không thể tự chỉnh sửa.
    ///         - `IsSubmit = false` (Mặc định): Chỉ lưu thông tin nháp, yêu cầu ở trạng thái "Đã lưu" để có thể tiếp tục chỉnh sửa sau.
    ///     - **Lưu ý về Tệp tin**: Các tệp tin tài liệu phải được tải lên trước thông qua API `POST api/upload-media` để lấy danh sách `Id`. Sau đó, sử dụng các `Id` này để điền vào trường `FileIds` trong danh sách `TaiLieuCuTrus`.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc hoàn cảnh**: 
    ///         - Luôn bắt buộc: `CanHoId`, `LoaiYeuCauId` (api/catalog/loai-yeu-cau-for-selector).
    ///         - Khi **Thêm mới**: Bắt buộc nhập đầy đủ `FirstName`, `LastName`, `Dob`, `GioiTinhId` (api/catalog/gioi-tinh-for-selector), `LoaiQuanHeId` (api/catalog/loai-quan-he-cu-tru-for-selector).
    ///         - Khi **Sửa/Xóa**: Bắt buộc cung cấp `TargetQuanHeCuTruId`.
    ///     - **Tùy chọn**: `CCCD`, `PhoneNumber`, `DiaChi`, `TaiLieuCuTrus`, `IsSubmit`.
    /// </remarks>
    [HttpPost("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauCuTruResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TaoYeuCau([FromBody] TaoYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật yêu cầu cư trú (Dành cho Cư dân)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cư dân bổ sung thông tin hoặc đính kèm tài liệu vào một yêu cầu cư trú trước khi gửi duyệt.
    /// - **Hệ thống xử lý**: Cập nhật chi tiết các thuộc tính yêu cầu và cho phép chuyển đổi trạng thái giữa "Đã lưu" và "Đang chờ duyệt" hoặc "Thu hồi".
    /// - **Cơ chế nộp và rút yêu cầu**:
    ///     - `IsSubmit = true`: Chốt dữ liệu và gửi cho BQL phê duyệt (chuyển từ "Đã lưu" sang "Chờ duyệt").
    ///     - `IsWithdraw = true`: Cư dân chủ động rút lại yêu cầu (chuyển sang trạng thái "Đã rút"). Hành động này ưu tiên hơn cập nhật nội dung.
    ///     - Nếu cả hai đều `false`: Chỉ cập nhật thay đổi nội dung và giữ yêu cầu ở trạng thái "Đã lưu".
    ///     - **Lưu ý về Tệp tin**: Các tệp tin tài liệu mới phải được tải lên trước thông qua API `POST api/upload-media` để lấy danh sách `Id`. Sau đó, sử dụng các `Id` này để điền vào trường `FileIds` trong danh sách `TaiLieuCuTrus`.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Id`.
    ///     - **Tùy chọn**: `FirstName`, `LastName`, `PhoneNumber`, `CCCD`, `DiaChi`, `TaiLieuCuTrus`, `IsSubmit`, `IsWithdraw`.
    /// </remarks>
    [HttpPut("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauCuTruResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapNhatYeuCau([FromBody] CapNhatYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa yêu cầu cư trú (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL dọn dẹp các yêu cầu rác hoặc yêu cầu bị nhầm lẫn.
    /// - **Hệ thống xử lý**: Thực hiện xóa cứng các bản ghi yêu cầu cư trú tương ứng.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Ids` (Danh sách ID yêu cầu).
    /// </remarks>
    [HttpDelete("yeu-cau")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> XoaYeuCau([FromBody] XoaYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Phê duyệt yêu cầu cư trú (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL phê duyệt yêu cầu hợp lệ của cư dân để cập nhật dữ liệu chính thức vào hệ thống.
    /// - **Hệ thống xử lý**: 
    ///     - **Thêm mới**: Tự động tạo Hồ sơ người dùng, lưu trữ tài liệu đính kèm và thiết lập quan hệ cư trú mới.
    ///     - **Chỉnh sửa**: Cập nhật thông tin cá nhân và quản lý tài liệu (thêm/sửa/xóa tài liệu cũ để khớp với yêu cầu).
    ///     - **Xóa**: Chấm dứt quan hệ cư trú hiện tại.
    ///     - Chuyển trạng thái yêu cầu sang "Đã duyệt" và ghi nhận người xử lý.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `YeuCauCuTruId`.
    /// </remarks>
    [HttpPost("yeu-cau/phe-duyet")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PheDuyetYeuCau([FromBody] PheDuyetYeuCauCuTruCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Từ chối yêu cầu cư trú (Dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL không chấp nhận yêu cầu của cư dân (thiếu hồ sơ, thông tin sai, v.v.).
    /// - **Hệ thống xử lý**: Ghi nhận lý do từ chối, chuyển trạng thái yêu cầu sang "Từ chối" và gửi phản hồi đến cư dân qua hệ thống thông báo.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `YeuCauCuTruId`, `LyDo`.
    /// </remarks>
    [HttpPost("yeu-cau/tu-choi")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
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
    /// - **Hoàn cảnh sử dụng**: BQL chuẩn bị quy trình bàn giao tài khoản ứng dụng cho cư dân một cách an toàn.
    /// - **Hệ thống xử lý**: Sinh mã định danh (Token) liên kết với UserId và có thời hạn xác thực ngắn hạn.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `QuanHeCuTruId`.
    /// </remarks>
    [HttpPost("tao-ma-dinh-danh")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TaoMaDinhDanh([FromBody] TaoMaDinhDanhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Trang giao diện xác nhận định danh (dành cho Cư dân - truy cập từ link email)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("xac-nhan-dinh-danh")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult XacNhanDinhDanhView([FromQuery] string token)
    {
        var postUrl = "/api/quan-he-cu-tru/xac-nhan-dinh-danh";
        return Content(IdentityHtmlTemplates.GetIdentificationProcessingPage(token, postUrl), "text/html");
    }

    /// <summary>
    /// Xác nhận định danh (dành cho Cư dân - qua link email)
    /// </summary>

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cư dân xác thực link từ Email để chính thức kết nối hồ sơ cư dân với tài khoản đăng nhập.
    /// - **Hệ thống xử lý**: Giải mã/Xác thực Token, thiết lập liên kết quan hệ và cập nhật quyền truy cập cho tài khoản.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Token`.
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("xac-nhan-dinh-danh")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> XacNhanDinhDanh([FromBody] XacNhanDinhDanhCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Liên kết tài khoản trực tiếp (dành cho BQL)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL thực hiện kết nối hồ sơ cư dân đã có với một tài khoản email đã đăng ký trên hệ thống.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra Email và User ID hợp lệ.
    ///     - Cập nhật liên kết trực tiếp trong cơ sở dữ liệu.
    ///     - Đồng bộ quyền hạn (Resident) cho tài khoản sau khi liên kết thành công.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `UserId`, `Email` (đúng định dạng email).
    /// </remarks>
    [HttpPost("lien-ket-tai-khoan")]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LienKetTaiKhoan([FromBody] LienKetTaiKhoanCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    #endregion
}
