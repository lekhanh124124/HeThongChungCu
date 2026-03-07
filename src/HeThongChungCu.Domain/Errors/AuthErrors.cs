namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "Email hoặc mật khẩu không chính xác.");

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
}
