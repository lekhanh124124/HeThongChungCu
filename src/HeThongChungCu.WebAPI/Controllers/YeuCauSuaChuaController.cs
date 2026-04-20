using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.UpdateYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.PheDuyetYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TuChoiYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.DieuPhoiNhanSu;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BoSungNhanSu;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.NhapBaoGiaYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HenLichYeuCauSuaChua;

using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HoanTatXuLyYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HuyYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.XoaNhanSuSuaChua;
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
    /// Cư dân tạo yêu cầu sửa chữa mới
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cư dân phát hiện sự cố và muốn yêu cầu BQL hỗ trợ sửa chữa.
    /// - **isSubmit = false**: Lưu nháp, cư dân có thể sửa/xóa sau, BQL chưa nhìn thấy.
    /// - **isSubmit = true**: Gửi ngay, khóa chỉnh sửa, yêu cầu xuất hiện trong hàng đợi BQL.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cư dân cập nhật yêu cầu sửa chữa đang ở trạng thái nháp
    /// </summary>
    /// <remarks>
    /// - **isSubmit = true**: Gửi yêu cầu sau khi cập nhật (Saved → Pending).
    /// - **isWithdraw = true**: Thu hồi yêu cầu đã gửi (Pending → Withdrawn).
    /// - Chỉ người tạo mới có quyền thực hiện.
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }


    /// <summary>
    /// BQL phê duyệt yêu cầu sửa chữa
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Phê duyệt yêu cầu sau khi cư dân gửi, cho phép bắt đầu bước điều phối nhân sự.
    /// - **Hệ thống xử lý**: Chuyển trạng thái sang "Approved" (Đã duyệt), ghi nhận người duyệt và ngày giờ.
    /// </remarks>
    [HttpPut("phe-duyet")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PheDuyet([FromBody] PheDuyetYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// BQL từ chối yêu cầu sửa chữa
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: BQL không chấp nhận yêu cầu của cư dân (ví dụ: yêu cầu không hợp lệ, không thuộc phạm vi xử lý).
    /// - **Hệ thống xử lý**: Chuyển trạng thái sang "Rejected" (Từ chối) và lưu lại lý do.
    /// </remarks>
    [HttpPut("tu-choi")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TuChoi([FromBody] TuChoiYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }


    /// <summary>
    /// Điều phối nhân sự (Nội bộ hoặc Đối tác)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Phân công kỹ thuật viên nội bộ hoặc đơn vị đối tác xử lý sự cố.
    /// - **Hệ thống xử lý**: 
    ///     - Nếu chọn đối tác: Gán hợp đồng, ghi nhận thợ thực hiện.
    ///     - Nếu chọn nội bộ: Gán danh sách KTV tòa nhà.
    ///     - Luôn xóa danh sách cũ trước khi gán mới. Chuyển trạng thái sang "DaDieuPhoi".
    /// </remarks>
    [HttpPut("dieu-phoi-nhan-su")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DieuPhoiNhanSu([FromBody] DieuPhoiNhanSuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Bổ sung nhân sự thực hiện
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi cần tăng cường thêm thợ đối tác hoặc KTV nội bộ mà không muốn xóa danh sách cũ.
    /// - **Hệ thống xử lý**: Thêm mới nhân sự, không xóa danh sách hiện tại. 
    /// - Phải cùng loại với nhân sự đã điều phối trước đó.
    /// </remarks>
    [HttpPut("bo-sung-nhan-su")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BoSungNhanSu([FromBody] BoSungNhanSuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa nhân sự tác nghiệp (Web)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi nhập sai thông tin thợ hoặc thợ đột xuất không thể tham gia, BQL thực hiện loại bỏ thợ khỏi danh sách.
    /// - **Hệ thống xử lý**: Thực hiện Soft Delete, lưu lại lý do xóa để phục vụ đối soát an ninh. 
    /// - **Ràng buộc**: Không cho phép xóa nhân sự cuối cùng nếu yêu cầu đã ở trạng thái Đã điều phối/Hẹn lịch/Báo giá.
    /// </remarks>
    [HttpDelete("xoa-nhan-su")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> XoaNhanSu([FromBody] XoaNhanSuSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Nhập báo giá sửa chữa (Web)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Kỹ thuật viên hoặc nhân viên BQL nhập chi phí dự kiến sau khi khảo sát.
    /// - **Hoàn cảnh nghiệp vụ**: BQL đã liên hệ cư dân để xác nhận giá trước, sau đó nhập và chốt luôn.
    /// - Ghi chú xác nhận từ cư dân vào `ghiChuBaoGia` (ví dụ: "Cư dân đồng ý xác nhận qua điện thoại ngày X").
    /// - **Hệ thống xử lý**: Luôn chuyển sang "DaDuyetBaoGia" (không còn bước chờ cư dân duyệt online).
    /// </remarks>
    [HttpPut("nhap-bao-gia")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> NhapBaoGia([FromBody] NhapBaoGiaYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }


    /// <summary>
    /// Hẹn lịch sửa chữa (Web)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Sau khi báo giá được duyệt, BQL hoặc kỹ thuật viên hẹn khung giờ cụ thể sẽ đến nhà cư dân.
    /// - **Hệ thống xử lý**: Cập nhật `HenTu`, `HenDen` và chuyển trạng thái sang "DaHenLich".
    /// </remarks>
    [HttpPut("hen-lich")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> HenLich([FromBody] HenLichYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Hoàn tất xử lý yêu cầu (Web)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi công việc sửa chữa hoàn thành, kỹ thuật viên ghi nhận kết quả và chi phí thực tế.
    /// - **Hệ thống xử lý**: Ghi nhận `KetQuaXuLy`, `ChiPhiThucTe`, chuyển trạng thái sang "DaDong" và phát sự kiện hoàn tất.
    /// - Có thể gọi trực tiếp từ trạng thái `DaDuyetBaoGia` hoặc `DaHenLich`.
    /// </remarks>
    [HttpPut("hoan-tat-xu-ly")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> HoanTatXuLy([FromBody] HoanTatXuLyYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Hủy yêu cầu sửa chữa (Web)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Ban quản lý muốn hủy yêu cầu vì lý do kỹ thuật hoặc cư dân yêu cầu qua kênh trực tiếp.
    /// - **Hệ thống xử lý**: Chuyển trạng thái sang "DaHuy".
    /// - **Lưu ý**: Chỉ dành cho Ban quản lý. Cư dân sử dụng "Thu hồi" trong API Cập nhật.
    /// </remarks>
    // [Authorize(Roles = "Staff,Admin")]
    [HttpDelete("huy")]
    [ProducesResponseType(typeof(ApiResponse<YeuCauSuaChuaDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Huy([FromBody] HuyYeuCauSuaChuaCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
