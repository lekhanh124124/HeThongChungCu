using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface INguoiDungDapperRepository
{
    Task<UserProfileDetailResponse?> GetByIdAsync(
        GetProfileSpecification spec, 
        CancellationToken cancellationToken = default);
}
