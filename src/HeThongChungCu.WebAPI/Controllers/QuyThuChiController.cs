using Asp.Versioning;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTaiChinh.Commands.TaoPhieuThu;
using HeThongChungCu.Application.Features.QLTaiChinh.Commands.TaoPhieuChi;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetNhatKyThuChi;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetQuyThuChiById;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportNhatKyThuChiExcel;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoThuChi;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoCanHo;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoToaNha;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportCongNoCanHoExcel;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportCongNoToaNhaExcel;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/quy-thu-chi")]
public class QuyThuChiController : ApiControllerBase
{
    private readonly ISender _sender;

    public QuyThuChiController(ISender sender)
    {
        _sender = sender;
    }

    // ──────────────────────────────────────────────
    // PHIẾU THU / CHI
    // ──────────────────────────────────────────────

    /// <summary>
    /// Tạo phiếu thu mới
    /// </summary>
    [HttpPost("tao-phieu-thu")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TaoPhieuThu([FromBody] TaoPhieuThuCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Tạo phiếu chi mới
    /// </summary>
    [HttpPost("tao-phieu-chi")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TaoPhieuChi([FromBody] TaoPhieuChiCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    // ──────────────────────────────────────────────
    // NHẬT KÝ QUỸ
    // ──────────────────────────────────────────────

    /// <summary>
    /// Truy vấn danh sách nhật ký quỹ thu/chi phân trang, hỗ trợ lọc và tìm kiếm
    /// </summary>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QuyThuChiResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList([FromBody] GetNhatKyThuChiQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy chi tiết thông tin phiếu thu/chi theo ID
    /// </summary>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<QuyThuChiResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromBody] GetQuyThuChiByIdQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Xuất file Excel nhật ký thu/chi quỹ
    /// </summary>
    [HttpPost("export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Export([FromBody] ExportNhatKyThuChiExcelQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return HandleResult(result);
        }
        return File(result.Value.Data, result.Value.ContentType, result.Value.FileName);
    }

    // ──────────────────────────────────────────────
    // BÁO CÁO TÀI CHÍNH
    // ──────────────────────────────────────────────

    /// <summary>
    /// Lấy báo cáo dòng tiền Thu - Chi tổng thể của Quỹ theo kỳ hạn thời gian
    /// </summary>
    [HttpPost("bao-cao/thu-chi")]
    [ProducesResponseType(typeof(ApiResponse<BaoCaoThuChiResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBaoCaoThuChi([FromBody] GetBaoCaoThuChiQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy báo cáo chi tiết công nợ dịch vụ của từng Căn hộ theo tháng/năm
    /// </summary>
    [HttpPost("bao-cao/cong-no-can-ho")]
    [ProducesResponseType(typeof(ApiResponse<List<BaoCaoCongNoCanHoResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBaoCaoCongNoCanHo([FromBody] GetBaoCaoCongNoCanHoQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy báo cáo tổng hợp công nợ và tỷ lệ thu hồi phí theo từng Tòa nhà theo tháng/năm
    /// </summary>
    [HttpPost("bao-cao/cong-no-toa-nha")]
    [ProducesResponseType(typeof(ApiResponse<List<BaoCaoCongNoToaNhaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBaoCaoCongNoToaNha([FromBody] GetBaoCaoCongNoToaNhaQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Xuất báo cáo chi tiết công nợ dịch vụ căn hộ ra file Excel
    /// </summary>
    [HttpPost("bao-cao/cong-no-can-ho/export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportCongNoCanHo([FromBody] ExportCongNoCanHoExcelQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return HandleResult(result);
        }
        return File(result.Value.Data, result.Value.ContentType, result.Value.FileName);
    }

    /// <summary>
    /// Xuất báo cáo tổng hợp công nợ tòa nhà ra file Excel
    /// </summary>
    [HttpPost("bao-cao/cong-no-toa-nha/export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportCongNoToaNha([FromBody] ExportCongNoToaNhaExcelQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return HandleResult(result);
        }
        return File(result.Value.Data, result.Value.ContentType, result.Value.FileName);
    }
}
