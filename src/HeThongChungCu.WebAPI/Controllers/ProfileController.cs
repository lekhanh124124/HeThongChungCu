using HeThongChungCu.Application.Features.Profile.Commands.ChangePassword;
using HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;
using HeThongChungCu.Application.Features.Profile.Commands.UpdateProfile;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ApiControllerBase
{
    private readonly ISender _sender;

    public ProfileController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Thay đổi mật khẩu cho người dùng đang đăng nhập
    /// </summary>
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Lấy thông tin cá nhân của người dùng đang đăng nhập
    /// </summary>
    [HttpPost("get-profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new GetProfileQuery(), cancellationToken));
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân của người dùng đang đăng nhập
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật ảnh đại diện của người dùng đang đăng nhập
    /// </summary>
    [HttpPost("change-avatar")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAvatar(IFormFile avatar, CancellationToken cancellationToken)
    {
        if (avatar == null || avatar.Length == 0)
        {
            return BadRequest(new ApiResponse<object>
            {
                IsOk = false,
                Errors = new List<Error> { new Error("File.Empty", "File is empty.") }
            });
        }

        await using var stream = avatar.OpenReadStream();

        var command = new UpdateAvatarCommand(
            stream,
            avatar.FileName,
            avatar.ContentType);

        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
