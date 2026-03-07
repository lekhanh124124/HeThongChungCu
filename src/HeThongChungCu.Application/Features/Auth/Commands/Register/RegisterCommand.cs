using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string IdCard,
    DateTime Dob,
    int GioiTinhId) : ICommand<AuthResponse>;
