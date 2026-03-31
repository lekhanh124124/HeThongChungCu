using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.UploadMedia.Commands.UploadFile;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/upload-media")]
public class UploadMediaController : ApiControllerBase
{
    private readonly ISender _sender;

    public UploadMediaController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Tải tệp tin lên hệ thống (Hình ảnh, Tài liệu)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cư dân hoặc BQL tải lên các hình ảnh (phương tiện, mặt trước/sau CCCD) hoặc tài liệu đính kèm trước khi thực hiện các yêu cầu nghiệp vụ chính.
    /// - **Hệ thống xử lý**: 
    ///     - Tiếp nhận danh sách tệp tin qua luồng dữ liệu `multipart/form-data`.
    ///     - Tải tệp lên dịch vụ lưu trữ đám mây.
    ///     - Lưu thông tin tệp vào cơ sở dữ liệu và trả về danh sách ID/URL. 
    ///     - **Lưu ý**: Client cần lưu lại các `Id` này để điền vào các yêu cầu nghiệp vụ tiếp theo (ví dụ: `HinhAnhIds`, `FileIds`, `TaiLieuCuTrus`).
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: Danh sách tệp tin (`files`) gửi qua `multipart/form-data`.
    ///     - **Tùy chọn**: `targetContainer` (string) - Chỉ định tên blob container để lưu trữ. Các giá trị hợp lệ hiện có:
    ///         - `tai-lieu-cu-tru` (Mặc định)
    ///         - `tai-lieu-phuong-tien` (Dành cho phương tiện)
    ///         - `anh-dai-dien-nguoi-dung`
    ///         - `hinh-anh-toa-nha`
    ///         - `hinh-anh-can-ho`
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<List<UploadFileResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromForm] string? targetContainer, List<IFormFile> files, CancellationToken cancellationToken)
    {
        var fileItems = files.Select(f => new FileUploadItem
        {
            Content = f.OpenReadStream(),
            FileName = f.FileName,
            ContentType = f.ContentType,
            Size = f.Length
        }).ToList();

        var command = new UploadFileCommand(fileItems, targetContainer);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
