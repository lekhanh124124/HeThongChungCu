using HeThongChungCu.Application.Features.ThongBao.DTOs;
using HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class ThongBaoQueryRepository : IThongBaoQueryRepository
{
    private readonly AppDbContext _dbContext;

    public ThongBaoQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ThongBaoResponse>> GetDSThongBaoAsync(LayDSThongBaoSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "pb.Id" },
            { "UserId", "pb.NguoiDungId" },
            { "IsRead", "pb.IsRead" },
            { "TieuDe", "t.TieuDe" },
            { "NoiDung", "t.NoiDung" },
            { "CreatedAt", "t.CreatedAt" },
        };

        var parameters = new DynamicParameters();
        var thongBaoQueryMap = new Dictionary<string, string> { { "ThongBaoIsDeleted", "t.IsDeleted" } };

        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("ThongBao", "t", "pb.ThongBaoId = t.Id", Mapping: thongBaoQueryMap)
        ], parameters);

        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "CreatedAt");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT 
                COUNT(*) OVER() AS TotalCount,
                pb.Id,
                pb.ThongBaoId,
                t.TieuDe,
                t.NoiDung,
                t.LoaiThongBao AS LoaiThongBaoId,
                t.ReferenceId,
                t.Metadata,
                pb.IsRead,
                t.CreatedAt,
                pb.ReadAt
            FROM PhanBoThongBao pb
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<ThongBaoReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;
        var loaiThongBaoMap = LoaiThongBao.ToDictionary();

        var items = rows.Select(r => new ThongBaoResponse
        {
            Id = r.Id,
            ThongBaoId = r.ThongBaoId,
            TieuDe = r.TieuDe,
            NoiDung = r.NoiDung,
            LoaiThongBaoId = r.LoaiThongBaoId,
            TenLoaiThongBao = loaiThongBaoMap.GetValueOrDefault(r.LoaiThongBaoId, string.Empty),
            ReferenceId = r.ReferenceId,
            Metadata = r.Metadata,
            IsRead = r.IsRead,
            CreatedAt = r.CreatedAt,
            ReadAt = r.ReadAt
        }).ToList();

        return new PagedResult<ThongBaoResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? 10,
                TotalItems = totalCount
            }
        };
    }
}
