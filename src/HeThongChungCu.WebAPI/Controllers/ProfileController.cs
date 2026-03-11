using HeThongChungCu.Application.Features.Profile.Commands.ChangePassword;
using HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;
using HeThongChungCu.Application.Features.Profile.Commands.UpdateProfile;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;
using HeThongChungCu.Application.Features.Profile.Queries.LayQuanHeCuTru;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ApiControllerBase
{
    private readonly ISender _sender;

    public ProfileController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy quan hệ cư trú của người dùng đang đăng nhập
    /// </summary>
    /// <remarks>
    /// API trả về danh sách các quan hệ cư trú đang hoạt động của người dùng, bao gồm thông tin căn hộ.
    /// </remarks>
    [HttpPost("quan-he-cu-tru")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LayQuanHeCuTruResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetQuanHeCuTru(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayQuanHeCuTruQuery(), cancellationToken));
    }

    /// <summary>
    /// Thay đổi mật khẩu cho người dùng đang đăng nhập
    /// </summary>
    /// <remarks>
    /// Yêu cầu người dùng đang đăng nhập cung cấp `CurrentPassword`, `NewPassword` và `ConfirmNewPassword`.
    /// Hệ thống sẽ kiểm tra mật khẩu hiện tại trước khi cập nhật sang mật khẩu mới.
    /// </remarks>
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
    /// <remarks>
    /// API trả về toàn bộ thông tin cá nhân (Profile) của user hiện tại đang được xác thực qua Access Token.
    /// Bao gồm thông tin cơ bản, căn hộ đang sinh sống, avatar, v.v.
    /// </remarks>
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
    /// <remarks>
    /// Cho phép người dùng chỉnh sửa các thông tin cá nhân như Họ Tên, Số điện thoại, CCCD, Ngày sinh, Giới tính, Địa chỉ.
    /// Trả về thông tin Profile mới nhất sau khi cập nhật.
    /// </remarks>
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
    /// <remarks>
    /// API dùng để upload file ảnh làm Avatar.
    /// Sử dụng Content-Type là `multipart/form-data`. Đối số `avatar` là file ảnh gửi lên.
    /// Trả về URL của ảnh đại diện mới.
    /// </remarks>
    [HttpPost("change-avatar")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAvatar(IFormFile? avatar, CancellationToken cancellationToken)
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
