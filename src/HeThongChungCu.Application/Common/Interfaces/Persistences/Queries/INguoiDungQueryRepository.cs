using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface INguoiDungQueryRepository
{
    Task<UserProfileDetailResponse?> GetByIdAsync(
        GetProfileSpecification spec, 
        CancellationToken cancellationToken = default);
}
