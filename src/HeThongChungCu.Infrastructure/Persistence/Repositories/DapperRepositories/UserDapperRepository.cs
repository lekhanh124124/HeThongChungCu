using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByPhoneNumber;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using System.Data;

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
            { "Id", "Id" },
            { "IsDeleted", "IsDeleted" }
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

    public async Task<SearchUserByUsernameResponse?> SearchResidentOrGuestByPhoneNumberAsync(
        GetUserByPhoneNumberSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {  "PhoneNumber", "u.PhoneNumber" },
            {  "RoleId", "u.RoleId" },
            {  "IsKetThuc", "q.IsKetThuc" },

            {  "UserIsDeleted", "u.IsDeleted" },
            {  "QuanHeCuTruIsDeleted", "q.IsDeleted" },

        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);

        var sql = $"""
            SELECT u.Id, u.Username, u.Email, u.FirstName, u.LastName, u.PhoneNumber, u.IdCard, u.Dob, u.GioiTinhId, u.RoleId
            FROM Users u
            INNER JOIN QuanHeCuTrus q ON u.Id = q.UserId
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
