using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Domain.Enums;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class UserDapperRepository : IUserDapperRepository
{
    private readonly AppDbContext _dbContext;

    public UserDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var transaction = _dbContext.GetDbTransaction();

        const string sql = """
            SELECT Id, Username, Email, FirstName, LastName, PhoneNumber, IdCard, Dob, DiaChi, GioiTinhId, RoleId, AnhDaiDienUrl
            FROM Users
            WHERE Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<UserProfileDetailResponse>(sql, new { Id = id });
    }

    public async Task<SearchUserByUsernameResponse?> SearchResidentOrGuestByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var transaction = _dbContext.GetDbTransaction();

        var roleKhach = Role.Guest.Value;
        var roleCuDan = Role.Resident.Value;

        const string sql = """
            SELECT Id, Username, Email, FirstName, LastName, PhoneNumber, IdCard, Dob, GioiTinhId, RoleId
            FROM Users
            WHERE Username = @Username AND (RoleId = @RoleKhach OR RoleId = @RoleCuDan)
            """;

        return await connection
            .QueryFirstOrDefaultAsync<SearchUserByUsernameResponse>(sql, 
                new 
                { 
                    Username = username, 
                    RoleKhach = roleKhach, 
                    RoleCuDan = roleCuDan 
                });
    }
}
