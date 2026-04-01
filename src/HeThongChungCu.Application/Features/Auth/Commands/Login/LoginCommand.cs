using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Username,
    string Password) : ICommand<AuthResponse>;
