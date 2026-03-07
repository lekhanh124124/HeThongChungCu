using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : ICommand<AuthResponse>;
