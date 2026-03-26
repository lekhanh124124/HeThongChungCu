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

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<List<UploadFileResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Upload(List<IFormFile> files, CancellationToken cancellationToken)
    {
        var fileItems = files.Select(f => new FileUploadItem
        {
            Content = f.OpenReadStream(),
            FileName = f.FileName,
            ContentType = f.ContentType,
            Size = f.Length
        }).ToList();

        var command = new UploadFileCommand(fileItems);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
