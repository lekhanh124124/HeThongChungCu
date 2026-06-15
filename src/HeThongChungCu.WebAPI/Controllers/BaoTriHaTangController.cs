using Asp.Versioning;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CapNhatTienDoBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateHangMucBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateLichBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreatePhieuBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateThietBi;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.DeleteHangMucBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.DeleteThietBi;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.HuyPhieuBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.KiemDuyetPhieuBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.QuetLichBaoTriVaSinhPhieu;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateHangMucBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateLichBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateThietBi;
using HeThongChungCu.Application.Features.BaoTriHaTang.Commands.PhanCongNhanSuBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriList;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriList;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.ExportPhieuBaoTri;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriList;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiList;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/bao-tri-ha-tang")]
public class BaoTriHaTangController : ApiControllerBase
{
    private readonly ISender _sender;

    public BaoTriHaTangController(ISender sender)
    {
        _sender = sender;
    }

    #region Thiết Bị (Devices)

    /// <summary>
    /// Tạo mới một thiết bị hạ tầng
    /// </summary>
    [HttpPost("thiet-bi")]
    [ProducesResponseType(typeof(ApiResponse<ThietBiResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateThietBi([FromBody] CreateThietBiCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin thiết bị hạ tầng
    /// </summary>
    [HttpPut("thiet-bi")]
    [ProducesResponseType(typeof(ApiResponse<ThietBiResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateThietBi([FromBody] UpdateThietBiCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa thiết bị hạ tầng
    /// </summary>
    [HttpDelete("thiet-bi")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteThietBi([FromBody] DeleteThietBiCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy thông tin chi tiết một thiết bị hạ tầng theo Id
    /// </summary>
    [HttpPost("thiet-bi/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<ThietBiDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetThietBiById([FromBody] GetThietBiByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách thiết bị hạ tầng (phân trang, lọc, tìm kiếm, sắp xếp)
    /// </summary>
    [HttpPost("thiet-bi/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ThietBiResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetThietBiList([FromBody] GetThietBiListQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    #endregion

    #region Lịch Bảo Trì (Schedules)

    /// <summary>
    /// Tạo mới một lịch bảo trì định kỳ cho thiết bị
    /// </summary>
    [HttpPost("lich-bao-tri")]
    [ProducesResponseType(typeof(ApiResponse<LichBaoTriResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateLichBaoTri([FromBody] CreateLichBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật lịch bảo trì định kỳ cho thiết bị
    /// </summary>
    [HttpPut("lich-bao-tri")]
    [ProducesResponseType(typeof(ApiResponse<LichBaoTriResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLichBaoTri([FromBody] UpdateLichBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết lịch bảo trì theo Id
    /// </summary>
    [HttpPost("lich-bao-tri/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<LichBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLichBaoTriById([FromBody] GetLichBaoTriByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách lịch bảo trì (phân trang, lọc, sắp xếp)
    /// </summary>
    [HttpPost("lich-bao-tri/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<LichBaoTriResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLichBaoTriList([FromBody] GetLichBaoTriListQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Quét lịch bảo trì định kỳ và tự động sinh phiếu bảo trì dự thảo
    /// </summary>
    [HttpPost("lich-bao-tri/quet-lich")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QuetLichBaoTri([FromBody] QuetLichBaoTriVaSinhPhieuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    #endregion

    #region Phiếu Bảo Trì (Tickets)

    /// <summary>
    /// Tạo mới một phiếu bảo trì hạ tầng
    /// </summary>
    [HttpPost("phieu-bao-tri")]
    [ProducesResponseType(typeof(ApiResponse<PhieuBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePhieuBaoTri([FromBody] CreatePhieuBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật tiến độ bảo trì thực tế (nhập checklist và vật tư)
    /// </summary>
    [HttpPut("phieu-bao-tri/cap-nhat-tien-do")]
    [ProducesResponseType(typeof(ApiResponse<PhieuBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CapNhatTienDo([FromBody] CapNhatTienDoBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Phân công nhân sự hoặc chỉ định đối tác cho phiếu bảo trì đang chờ giao việc
    /// </summary>
    [HttpPost("phieu-bao-tri/phan-cong")]
    [ProducesResponseType(typeof(ApiResponse<PhieuBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PhanCong([FromBody] PhanCongNhanSuBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Kiểm duyệt và hoàn tất nghiệm thu phiếu bảo trì hạ tầng
    /// </summary>
    [HttpPut("phieu-bao-tri/kiem-duyet")]
    [ProducesResponseType(typeof(ApiResponse<PhieuBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> KiemDuyet([FromBody] KiemDuyetPhieuBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Hủy bỏ một phiếu bảo trì hạ tầng
    /// </summary>
    [HttpPut("phieu-bao-tri/huy")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> HuyPhieu([FromBody] HuyPhieuBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy thông tin chi tiết một phiếu bảo trì hạ tầng theo Id
    /// </summary>
    [HttpPost("phieu-bao-tri/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<PhieuBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPhieuBaoTriById([FromBody] GetPhieuBaoTriByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách phiếu bảo trì hạ tầng (phân trang, lọc, sắp xếp, tìm kiếm)
    /// </summary>
    [HttpPost("phieu-bao-tri/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PhieuBaoTriResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPhieuBaoTriList([FromBody] GetPhieuBaoTriListQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Xuất file Excel phiếu bảo trì định dạng in ấn thực địa (Print-ready)
    /// </summary>
    [HttpPost("phieu-bao-tri/export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportPhieuBaoTri([FromBody] ExportPhieuBaoTriQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return HandleResult(result);
        }
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    #endregion

    #region Hạng Mục Bảo Trì (Maintenance Items)

    /// <summary>
    /// Tạo mới một hạng mục bảo trì hạ tầng
    /// </summary>
    [HttpPost("hang-muc")]
    [ProducesResponseType(typeof(ApiResponse<HangMucBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateHangMucBaoTri([FromBody] CreateHangMucBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin hạng mục bảo trì hạ tầng
    /// </summary>
    [HttpPut("hang-muc")]
    [ProducesResponseType(typeof(ApiResponse<HangMucBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateHangMucBaoTri([FromBody] UpdateHangMucBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa hạng mục bảo trì hạ tầng
    /// </summary>
    [HttpDelete("hang-muc")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteHangMucBaoTri([FromBody] DeleteHangMucBaoTriCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy thông tin chi tiết một hạng mục bảo trì theo Id
    /// </summary>
    [HttpPost("hang-muc/get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<HangMucBaoTriDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHangMucBaoTriById([FromBody] GetHangMucBaoTriByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách hạng mục bảo trì (phân trang, lọc, sắp xếp, tìm kiếm)
    /// </summary>
    [HttpPost("hang-muc/get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<HangMucBaoTriResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHangMucBaoTriList([FromBody] GetHangMucBaoTriListQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    #endregion
}
