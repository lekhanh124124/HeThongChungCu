using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Queries.GetProfile;

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, UserProfileDetailResponse>
{
    private readonly IUserDapperRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetProfileQueryHandler(IUserDapperRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserProfileDetailResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return Result.Failure<UserProfileDetailResponse>(AuthErrors.InvalidCredentials);
        }

        var profile = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (profile is null)
        {
            return Result.Failure<UserProfileDetailResponse>(AuthErrors.InvalidCredentials);
        }

        // Map Enum names
        profile.GioiTinhName = GioiTinh.FromValue(profile.GioiTinhId)?.Name ?? string.Empty;
        profile.RoleName = Role.FromValue(profile.RoleId)?.Name ?? string.Empty;

        return Result.Success(profile);
    }
}
