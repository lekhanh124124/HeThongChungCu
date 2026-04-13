using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Domain.Enums;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class NguoiDungQueryRepository : INguoiDungQueryRepository
{
    private readonly AppDbContext _dbContext;

    public NguoiDungQueryRepository(AppDbContext dbContext)
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
            { "Id", "u.Id" },
            { "IsDeleted", "u.IsDeleted" }
        };

        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
        var joins = new[]
        {
            new JoinDefinition("TaiKhoan", "a", "u.Id = a.NguoiDungId AND a.IsActive = 1", AddSoftDelete: false),
            new JoinDefinition("TepTaiLieu", "atl", "a.AnhDaiDienId = atl.Id"),
            new JoinDefinition("PhanQuyen", "pq", "a.Id = pq.TaiKhoanId", AddSoftDelete: false)
        };
        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);

        var sql = $"""
            SELECT u.Id, a.TenDangNhap as Username, a.Email, u.Ten as FirstName, u.Ho as LastName, u.SoDienThoai as PhoneNumber, u.NgaySinh as Dob, u.DiaChi, u.GioiTinhId, atl.FileUrl as AnhDaiDienUrl,
                   STRING_AGG(pq.RoleId, ',') as RoleIds
            FROM NguoiDung u
            {sqlJoins}
            {sqlWhere}
            GROUP BY u.Id, a.TenDangNhap, a.Email, u.Ten, u.Ho, u.SoDienThoai, u.NgaySinh, u.DiaChi, u.GioiTinhId, atl.FileUrl
            """;

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        if (result is null) return null;

        var user = new UserProfileDetailResponse
        {
            Id = result.Id,
            Username = result.Username ?? string.Empty,
            Email = result.Email,
            FirstName = result.FirstName,
            LastName = result.LastName,
            PhoneNumber = result.PhoneNumber,
            Dob = result.Dob,
            DiaChi = result.DiaChi,
            GioiTinhId = result.GioiTinhId,
            AnhDaiDienUrl = result.AnhDaiDienUrl ?? string.Empty
        };

        var gioiTinhMap = GioiTinh.ToDictionary();
        user.GioiTinhName = gioiTinhMap.GetValueOrDefault(user.GioiTinhId, string.Empty);

        if (!string.IsNullOrEmpty(result.RoleIds))
        {
            var roleMap = Role.ToDictionary();
            var roleIds = ((string)result.RoleIds).Split(',').Select(int.Parse);
            user.Roles = roleIds.Select(id => roleMap.GetValueOrDefault(id, string.Empty)).ToList();
        }

        return user;
    }
}
