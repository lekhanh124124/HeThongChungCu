using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.Auth.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class UserDapperRepository : DapperDbContext, IUserDapperRepository
{
    public UserDapperRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<UserProfileDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            const string sql = """
            SELECT Id, Username, Email, FirstName, LastName, PhoneNumber, IdCard, Dob, GioiTinhId, RoleId
            FROM Users
            WHERE Id = @Id
            """;

            return await connection.QueryFirstOrDefaultAsync<UserProfileDetailResponse>(sql, new { Id = id });
        }
    }

    public async Task<SearchUserByUsernameResponse?> SearchResidentOrGuestByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var roleKhach = Role.Guest.Value;
        var roleCuDan = Role.Resident.Value;

        using (var connection = CreateConnection())
        {
            const string sql = """
            SELECT Id, Username, Email, FirstName, LastName, PhoneNumber, IdCard, Dob, GioiTinhId, RoleId
            FROM Users
            WHERE Username = @Username AND (RoleId = @RoleKhach OR RoleId = @RoleCuDan)
            """;

            return await connection.QueryFirstOrDefaultAsync<SearchUserByUsernameResponse>(sql, new { Username = username, RoleKhach = roleKhach, RoleCuDan = roleCuDan });
        }
    }
}
