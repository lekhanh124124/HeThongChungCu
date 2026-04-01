namespace HeThongChungCu.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Username) : ICommand<string>;
