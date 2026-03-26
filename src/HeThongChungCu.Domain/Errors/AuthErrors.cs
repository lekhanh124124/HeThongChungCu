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
        "Token không chính xác.");

    public static readonly Error AccountLocked = new(
         "Auth.AccountLocked",
         "Tài khoản bị khóa.");

    public static readonly Error Unauthorized = new(
        "Auth.Unauthorized",
        "Bạn chưa đăng nhập.");

    public static readonly Error Forbidden = new(
        "Auth.Forbidden",
        "Bạn không có quyền truy cập tài nguyên này.");

    public static readonly Error PasswordNotChanged = new(
        "Auth.PasswordNotChanged",
        "Mật khẩu mới không được giống mật khẩu cũ.");

    public static readonly Error AccountNotFound = new(
        "Auth.AccountNotFound",
        "Tài khoản không tồn tại.");
}
