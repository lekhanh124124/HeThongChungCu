using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ConfirmChiSoBatch;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ImportChiSo;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.RecordChiSoBatch;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UpdateChiSoTieuThu;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.DeleteChiSoTieuThu;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UploadChiSoImagesBatch;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetChiSoById;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetDichVuTieuThu;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/chi-so-tieu-thu")]
public class ChiSoTieuThuController : ApiControllerBase
{
    private readonly ISender _sender;

    public ChiSoTieuThuController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách chỉ số tiêu thụ
    /// </summary>
    [HttpPost("get-list")]
    public async Task<IActionResult> GetList([FromBody] GetListChiSoQuery query)
    {
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy danh sách dịch vụ có sử dụng chỉ số tiêu thụ (Điện, Nước...)
    /// </summary>
    [HttpPost("get-list-dich-vu-tieu-thu")]
    public async Task<IActionResult> GetDichVuTieuThu()
    {
        var result = await _sender.Send(new GetDichVuTieuThuQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy chi tiết chỉ số tiêu thụ
    /// </summary>
    [HttpPost("get-by-id")]
    public async Task<IActionResult> GetById([FromBody] GetChiSoByIdQuery query)
    {
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Xuất file Excel mẫu để ghi nhận chỉ số
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] ExportChiSoTemplateQuery query)
    {
        var result = await _sender.Send(query);
        if (result.IsFailure) return HandleResult(result);

        return File(result.Value.Data, result.Value.ContentType, result.Value.FileName);
    }

    /// <summary>
    /// Nhập chỉ số tiêu thụ từ file Excel
    /// </summary>
    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file, [FromForm] int thang, [FromForm] int nam, [FromForm] DateTimeOffset ngayChot)
    {
        var command = new ImportChiSoCommand(file.OpenReadStream(), thang, nam, ngayChot);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Ghi nhận chỉ số hàng loạt (Thủ công)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Record([FromBody] RecordChiSoBatchCommand command)
    {
        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Xác nhận hàng loạt chỉ số để sẵn sàng lập hóa đơn
    /// </summary>
    [HttpPut("xac-nhan")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmChiSoBatchCommand command)
    {
        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Upload file zip chứa hàng loạt ảnh đồng hồ
    /// </summary>
    [HttpPost("import-images")]
    public async Task<IActionResult> UploadImagesBatch(IFormFile zipFile)
    {
        var command = new UploadChiSoImagesBatchCommand(zipFile.OpenReadStream(), zipFile.FileName);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Cập nhật chỉ số tiêu thụ
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateChiSoTieuThuCommand command)
    {
        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Xóa danh sách chỉ số tiêu thụ
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteChiSoTieuThuCommand command)
    {
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}
