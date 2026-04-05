namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "Tên đăng nhập hoặc mật khẩu không chính xác.");

    public static readonly Error InvalidOldPassword = new(
        "Auth.InvalidPassword",
        "Mật khẩu cũ không chính xác.");

    public static readonly Error InvalidRefreshToken = new(
        "Auth.InvalidRefreshToken",
        "Refresh token không chính xác hoặc đã hết hạn.");

    public static readonly Error InvalidResetToken = new(
        "Auth.InvalidResetToken",
        "Mã khôi phục không chính xác hoặc đã hết hạn.");

    public static readonly Error InvalidToken = new(
        "Auth.InvalidToken",
        "Mã xác nhận không hợp lệ hoặc đã hết hạn.");

    public static readonly Error AccountAlreadyLinked = new(
        "Auth.AccountAlreadyLinked",
        "Tài khoản này đã được liên kết với một người dùng khác.");

    public static readonly Error ResidentAlreadyLinked = new(
        "Auth.ResidentAlreadyLinked",
        "Thông tin định danh này đã được liên kết với một tài khoản khác.");

    public static readonly Error AccountLocked = new(
         "Auth.AccountLocked",
         "Tài khoản bị khóa.");

    public static readonly Error Unauthorized = new(
        "Auth.Unauthorized",
        "Bạn chưa đăng nhập.");

    public static readonly Error TokenExpired = new(
        "Auth.TokenExpired",
        "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.");

    public static readonly Error Forbidden = Error.Forbidden("truy cập tài nguyên này");
    
    public static readonly Error PasswordNotChanged = new(
        "Auth.PasswordNotChanged",
        "Mật khẩu mới không được giống mật khẩu cũ.");

    public static readonly Error AccountNotFound = Error.NotFound("Tài khoản");

    public static readonly Error PasswordRequiresUpper = new(
        "Auth.PasswordRequiresUpper",
        "Mật khẩu phải chứa ít nhất một chữ cái viết hoa.");

    public static readonly Error PasswordRequiresLower = new(
        "Auth.PasswordRequiresLower",
        "Mật khẩu phải chứa ít nhất một chữ cái viết thường.");

    public static readonly Error PasswordRequiresDigit = new(
        "Auth.PasswordRequiresDigit",
        "Mật khẩu phải chứa ít nhất một chữ số.");

    public static readonly Error PasswordRequiresNonAlphanumeric = new(
        "Auth.PasswordRequiresNonAlphanumeric",
        "Mật khẩu phải chứa ít nhất một ký tự đặc biệt.");

    public static readonly Error PasswordConfirmationMismatch = new(
        "Auth.PasswordConfirmationMismatch",
        "Xác nhận mật khẩu phải khớp với mật khẩu.");

    public static readonly Error EmailNotEmpty = Error.NotEmpty("Email");
    public static readonly Error EmailInvalid = Error.InvalidEmail("Email");
    public static readonly Error PasswordNotEmpty = Error.NotEmpty("Mật khẩu");
    public static Error PasswordMinLength(int length) => Error.MinLength("Mật khẩu", length);
    public static readonly Error ConfirmPasswordNotEmpty = Error.NotEmpty("Xác nhận mật khẩu");
    public static readonly Error UsernameNotEmpty = Error.NotEmpty("Tên đăng nhập");
    public static readonly Error ResetCodeNotEmpty = Error.NotEmpty("Mã khôi phục");
    public static readonly Error NewPasswordNotEmpty = Error.NotEmpty("Mật khẩu mới");
    public static readonly Error CurrentPasswordNotEmpty = Error.NotEmpty("Mật khẩu hiện tại");
    public static readonly Error RefreshTokenNotEmpty = Error.NotEmpty("Refresh token");
}
