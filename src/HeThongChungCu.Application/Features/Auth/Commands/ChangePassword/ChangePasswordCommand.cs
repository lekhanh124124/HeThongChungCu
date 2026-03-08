namespace HeThongChungCu.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
    string OldPassword,
    string NewPassword,
    string ConfirmPassword) : ICommand<string>;
