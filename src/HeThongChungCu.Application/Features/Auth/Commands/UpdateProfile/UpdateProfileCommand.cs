using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string IdCard,
    DateTime Dob,
    int GioiTinhId) : ICommand<UserProfileDetailResponse>;
