namespace HeThongChungCu.Application.Features.Profile.Commands.ChangePassword;

public record ChangePasswordCommand(
    string OldPassword,
    string NewPassword,
    string ConfirmPassword) : ICommand<string>;
