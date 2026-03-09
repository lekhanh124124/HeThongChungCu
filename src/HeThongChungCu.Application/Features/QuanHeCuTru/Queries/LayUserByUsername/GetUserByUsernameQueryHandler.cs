using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByUsername;

public class GetUserByUsernameQueryHandler : IQueryHandler<GetUserByUsernameQuery, SearchUserByUsernameResponse>
{
    private readonly IUserDapperRepository _userRepository;

    public GetUserByUsernameQueryHandler(IUserDapperRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<SearchUserByUsernameResponse>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.SearchResidentOrGuestByUsernameAsync(request.Username, cancellationToken);

        if (user is null)
        {
            return Result.Failure<SearchUserByUsernameResponse>(UserErrors.NotFoundByUsername(request.Username));
        }

        user.GioiTinhName = GioiTinh.FromValue(user.GioiTinhId)?.Name ?? string.Empty;
        user.RoleName = Role.FromValue(user.RoleId)?.Name ?? string.Empty;

        return Result.Success(user);
    }
}
