using HeThongChungCu.Application.Features.Profile.DTOs;

namespace HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

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

        return Result.Success(profile);
    }
}
