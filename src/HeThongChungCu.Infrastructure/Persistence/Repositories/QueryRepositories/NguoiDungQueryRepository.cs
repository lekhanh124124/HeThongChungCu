using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

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
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "u.Id" },
            { "IsDeleted", "u.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters, addSoftDeleteFilter: true);

        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("TaiKhoan", "a", "u.Id = a.NguoiDungId AND a.IsActive = 1", JoinType.Left, true),
            new JoinDefinition("TepTaiLieu", "atl", "a.AnhDaiDienId = atl.Id", JoinType.Left, true, Discriminators: [("LoaiTepTaiLieu", "TepTaiLieu")])
        ]);

        var sqlJoinsPq = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("TaiKhoan", "a", "u.Id = a.NguoiDungId AND a.IsActive = 1", JoinType.Left, true),
            new JoinDefinition("PhanQuyen", "pq", "pq.TaiKhoanId = a.Id", JoinType.Left, false)
        ]);

        var sql = $"""
            SELECT u.Id, a.TenDangNhap as Username, a.Email, u.Ten as FirstName, u.Ho as LastName, u.SoDienThoai as PhoneNumber, u.NgaySinh as Dob, u.DiaChi, u.GioiTinhId, atl.FileUrl as AnhDaiDienUrl
            FROM NguoiDung u
            {sqlJoins}
            {sqlWhere};

            SELECT pq.RoleId
            FROM NguoiDung u
            {sqlJoinsPq}
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());

        var result = await multi.ReadFirstOrDefaultAsync<UserProfileReadModel>();
        if (result is null) return null;

        var roleIds = (await multi.ReadAsync<int>()).ToList();

        var user = new UserProfileDetailResponse
        {
            Id = result.Id,
            Username = result.Username,
            Email = result.Email,
            FirstName = result.FirstName,
            LastName = result.LastName,
            PhoneNumber = result.PhoneNumber,
            Dob = result.Dob.GetValueOrDefault(),
            DiaChi = result.DiaChi ?? string.Empty,
            GioiTinhId = result.GioiTinhId,
            AnhDaiDienUrl = result.AnhDaiDienUrl ?? string.Empty
        };

        var gioiTinhMap = GioiTinh.ToDictionary();
        user.GioiTinhName = gioiTinhMap.GetValueOrDefault(user.GioiTinhId, string.Empty);

        if (roleIds.Any())
        {
            var roleMap = Role.ToDictionary();
            user.Roles = roleIds.Select(id => roleMap.GetValueOrDefault(id, string.Empty)).ToList();
        }

        return user;
    }
}
