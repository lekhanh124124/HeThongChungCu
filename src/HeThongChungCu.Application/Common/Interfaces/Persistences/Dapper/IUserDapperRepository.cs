using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByUsername;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IUserDapperRepository
{
    Task<UserProfileDetailResponse?> GetByIdAsync(GetProfileSpecification spec, CancellationToken cancellationToken = default);
    Task<SearchUserByUsernameResponse?> SearchResidentOrGuestByUsernameAsync(
        GetUserByUsernameSpecification spec,
        CancellationToken cancellationToken = default);
}
