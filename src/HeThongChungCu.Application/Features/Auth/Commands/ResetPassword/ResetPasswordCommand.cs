namespace HeThongChungCu.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Username,
    string ResetCode,
    string NewPassword,
    string ConfirmPassword) : ICommand<string>;
