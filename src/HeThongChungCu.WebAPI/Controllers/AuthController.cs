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
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
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
    /// - **Hoàn cảnh sử dụng**: Người dùng bắt đầu phiên làm việc mới bằng cách cung cấp thông tin định danh.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra sự tồn tại của tài khoản và xác thực mật khẩu (đã hash).
    ///     - Sinh cặp mã JWT Access Token (ngắn hạn) và Refresh Token (dài hạn).
    ///     - Trả về thông tin cơ bản của người dùng và các quyền (Roles) tương ứng.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Username`, `Password`.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Đăng ký tài khoản mới (Khách)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Người dùng mới (khách) tự đăng ký tài khoản để truy cập các tính năng cơ bản của hệ thống.
    /// - **Hệ thống xử lý**: 
    ///     - Tạo tài khoản mới với vai trò "Khách" (Guest).
    ///     - Mã hóa mật khẩu bảo mật và lưu trữ.
    ///     - Tự động đăng nhập và trả về mã xác thực sau khi đăng ký thành công.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Email`, `Password`, `ConfirmPassword`.
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
    /// - **Hoàn cảnh sử dụng**: Khi Access Token hết hạn, ứng dụng client sử dụng Refresh Token để lấy Access Token mới mà không yêu cầu người dùng đăng nhập lại.
    /// - **Hệ thống xử lý**: 
    ///     - Kiểm tra tính hợp lệ và thời hạn của Refresh Token trong cơ sở dữ liệu.
    ///     - Thực hiện cơ chế xoay vòng Token (Rotation) để đảm bảo an toàn.
    ///     - Sinh Access Token mới nếu Refresh Token còn hiệu lực.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `RefreshToken`.
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
    /// - **Yêu cầu dữ liệu**: Không có.
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
    /// - **Hoàn cảnh sử dụng**: Người dùng không nhớ mật khẩu và yêu cầu hệ thống hỗ trợ khôi phục.
    /// - **Hệ thống xử lý**: 
    ///     - Tìm kiếm tài khoản dựa trên Username/Email.
    ///     - Sinh mã khôi phục (Reset Code) ngẫu nhiên, mã hóa và lưu trữ kèm thời gian hết hạn (ví dụ: 10 phút).
    ///     - Gửi mã này đến Email đăng ký của người dùng.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Username`.
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
    /// - **Hoàn cảnh sử dụng**: Người dùng nhập mã khôi phục nhận được từ Email để thiết lập mật khẩu mới.
    /// - **Hệ thống xử lý**: 
    ///     - Xác thực mã khôi phục (hợp lệ, chưa hết hạn, đúng tài khoản).
    ///     - Mã hóa mật khẩu mới và cập nhật vào cơ sở dữ liệu.
    ///     - Thu hồi (Revoke) mã khôi phục ngay sau khi sử dụng thành công để đảm bảo an ninh.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: `Username`, `ResetCode`, `NewPassword`, `ConfirmNewPassword`.
    /// </remarks>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}
