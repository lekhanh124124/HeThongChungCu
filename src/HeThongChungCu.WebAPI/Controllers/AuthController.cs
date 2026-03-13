using HeThongChungCu.Application.Features.Auth.Commands.ForgotPassword;
using HeThongChungCu.Application.Features.Auth.Commands.Login;
using HeThongChungCu.Application.Features.Auth.Commands.Logout;
using HeThongChungCu.Application.Features.Auth.Commands.RefreshToken;
using HeThongChungCu.Application.Features.Auth.Commands.Register;
using HeThongChungCu.Application.Features.Auth.Commands.ResetPassword;
using HeThongChungCu.Application.Features.Auth.DTOs;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Đăng nhập để nhận JWT và Refresh Token
    /// </summary>
    /// <remarks>
    /// API cung cấp tính năng xác thực người dùng. 
    /// - Yêu cầu đầu vào: Username và Password.
    /// - Kết quả: Trả về Access Token (thời hạn ngắn, dùng để gọi các API yêu cầu quyền) và Refresh Token (thời hạn dài, dùng để cấp lại Access Token khi hết hạn).
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Đăng ký tài khoản mới cho cư dân
    /// </summary>
    /// <remarks>
    /// API dùng để tạo tài khoản mới trên hệ thống. 
    /// Mặc định, tài khoản được tạo thông qua API này sẽ được gán role `Resident` (Cư dân).
    /// Các trường bắt buộc bao gồm: Username, Email, Password, FirstName, LastName, PhoneNumber, IdCard, Dob, GioiTinhId, và DiaChi.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cấp lại Access Token mới bằng Refresh Token
    /// </summary>
    /// <remarks>
    /// API này được gọi khi Access Token đã hết hạn.
    /// Yêu cầu cung cấp `RefreshToken` hợp lệ, chưa bị thu hồi.
    /// Hệ thống sẽ vô hiệu hóa Refresh Token cũ và tạo ra một cặp Access Token và Refresh Token mới để duy trì đăng nhập.
    /// </remarks>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Đăng xuất và thu hồi Refresh Token
    /// </summary>
    /// <remarks>
    /// API dùng để đăng xuất người dùng hiện tại khỏi hệ thống.
    /// Yêu cầu người dùng phải đang đăng nhập (gửi kèm Bearer Token). 
    /// Hệ thống sẽ tìm và vô hiệu hóa (revoke) Refresh Token hiện tại đang được sử dụng gắn với tài khoản này để ngăn chặn việc cấp lại Access Token trong tương lai.
    /// </remarks>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LogoutCommand(), cancellationToken));
    }

    /// <summary>
    /// Yêu cầu tạo mã khôi phục mật khẩu gửi qua email
    /// </summary>
    /// <remarks>
    /// API dùng cho trường hợp người dùng quên mật khẩu.
    /// Gửi lên tên đăng nhập của người dùng. Hệ thống sẽ kiểm tra và nếu tồn tại, sẽ sinh ra một mã khôi phục (OTP/Token) và gửi vào hộp thư Email của người dùng đó.
    /// </remarks>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Đặt lại mật khẩu mới thông qua mã khôi phục
    /// </summary>
    /// <remarks>
    /// Sau khi nhận được mã khôi phục qua email, người dùng sử dụng API này để thiết lập mật khẩu mới.
    /// Yêu cầu cung cấp `Email`, `ResetCode` (mã từ email), `NewPassword` và `ConfirmPassword`.
    /// </remarks>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
