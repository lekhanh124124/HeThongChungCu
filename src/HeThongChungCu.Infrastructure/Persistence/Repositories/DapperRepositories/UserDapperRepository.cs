using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByUsername;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using System.Data;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.DapperRepositories;

public class UserDapperRepository : IUserDapperRepository
{
    private readonly AppDbContext _dbContext;

    public UserDapperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileDetailResponse?> GetByIdAsync(GetProfileSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(User.Id), "Id" },
            { nameof(User.IsDeleted), "IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT Id, Username, Email, FirstName, LastName, PhoneNumber, IdCard, Dob, DiaChi, GioiTinhId, RoleId, AnhDaiDienUrl
            FROM Users
            {sqlWhere}
            """;

        var user = await connection.QueryFirstOrDefaultAsync<UserProfileDetailResponse>(sql, parameters);

        if (user is not null)
        {
            var gioiTinhMap = GioiTinh.ToDictionary();
            var roleMap = Role.ToDictionary();
            user.GioiTinhName = gioiTinhMap.GetValueOrDefault(user.GioiTinhId, string.Empty);
            user.RoleName = roleMap.GetValueOrDefault(user.RoleId, string.Empty);
        }

        return user;
    }

    public async Task<SearchUserByUsernameResponse?> SearchResidentOrGuestByUsernameAsync(
        GetUserByUsernameSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {  nameof(User.Username), "Username" },
            {  nameof(User.RoleId), "RoleId" },
            {  nameof(User.IsDeleted), "IsDeleted" }

        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT Id, Username, Email, FirstName, LastName, PhoneNumber, IdCard, Dob, GioiTinhId, RoleId
            FROM Users
            {sqlWhere}
            """;

        var user = await connection.QueryFirstOrDefaultAsync<SearchUserByUsernameResponse>(sql, parameters);

        if (user is not null)
        {
            var gioiTinhMap = GioiTinh.ToDictionary();
            var roleMap = Role.ToDictionary();
            user.GioiTinhName = gioiTinhMap.GetValueOrDefault(user.GioiTinhId, string.Empty);
            user.RoleName = roleMap.GetValueOrDefault(user.RoleId, string.Empty);
        }

        return user;
    }
}
