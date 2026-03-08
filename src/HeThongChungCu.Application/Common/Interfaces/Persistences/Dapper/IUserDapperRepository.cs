using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IUserDapperRepository
{
    Task<UserProfileDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
