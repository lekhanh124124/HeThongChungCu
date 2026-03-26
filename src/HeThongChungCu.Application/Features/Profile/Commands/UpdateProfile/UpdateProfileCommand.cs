using HeThongChungCu.Application.Features.Profile.DTOs;

namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateTime Dob,
    int GioiTinhId,
    string DiaChi) : ICommand<UserProfileDetailResponse>;
