using HeThongChungCu.Application.Features.Auth.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IUserDapperRepository
{
    Task<UserProfileDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SearchUserByUsernameResponse?> SearchResidentOrGuestByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
