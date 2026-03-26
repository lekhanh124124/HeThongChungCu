using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword) : ICommand<AuthResponse>;
