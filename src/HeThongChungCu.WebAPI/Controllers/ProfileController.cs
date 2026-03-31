using HeThongChungCu.Application.Features.Profile.Commands.ChangePassword;
using HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;
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
    /// Thay đổi mật khẩu cho người dùng đang đăng nhập
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Người dùng đang đăng nhập muốn cập nhật lại mật khẩu để tăng cường bảo mật hoặc theo định kỳ.
    /// - **Hệ thống xử lý**: 
    ///     - Xác thực mật khẩu cũ bằng cách so sánh hash.
    ///     - Kiểm tra tính hợp lệ của mật khẩu mới (không trùng mật khẩu cũ, khớp với xác nhận).
    ///     - Mã hóa mật khẩu mới và cập nhật vào cơ bản dữ liệu tài khoản.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `OldPassword`, `NewPassword`, `ConfirmPassword`.
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
    /// - **Hoàn cảnh sử dụng**: Ứng dụng client lấy thông tin chi tiết của người dùng hiện tại để hiển thị trên trang cá nhân hoặc các thành phần giao diện liên quan.
    /// - **Hệ thống xử lý**: 
    ///     - Trích xuất UserId từ thông tin định danh (Claims) của người dùng hiện hành.
    ///     - Truy xuất toàn bộ thông tin hồ sơ (User Profile) và thông tin tài khoản (Account) liên kết.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("get-profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new GetProfileQuery(), cancellationToken));
    }


    /// <summary>
    /// Cập nhật ảnh đại diện của người dùng đang đăng nhập
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Người dùng thay đổi ảnh đại diện cá nhân.
    /// - **Hệ thống xử lý**: 
    ///     - Tiếp nhận luồng dữ liệu file từ yêu cầu `multipart/form-data`.
    ///     - Tải tệp lên dịch vụ lưu trữ (Cloud Storage) và cập nhật đường dẫn URL vào hồ sơ người dùng.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: File ảnh (`avatar`) gửi qua `multipart/form-data`.
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
