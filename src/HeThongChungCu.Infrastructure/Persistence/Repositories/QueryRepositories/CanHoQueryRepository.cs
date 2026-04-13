using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;
using HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class CanHoQueryRepository : ICanHoQueryRepository
{
    private readonly AppDbContext _dbContext;
    public CanHoQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CanHoDetailResponse>> GetAllAsync(
        GetListCanHoSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "c.Id" },
            { "MaCanHo", "c.MaCanHo" },
            { "DienTich", "c.DienTich" },
            { "SoPhongNgu", "c.SoPhongNgu" },
            { "SoPhongTam", "c.SoPhongTam" },
            { "TinhTrangCanHoId", "c.TinhTrangCanHoId" },
            { "TangId", "c.TangId" },
            { "TenTang", "t.TenTang" },
            { "TenCanHo", "c.TenCanHo" },
            { "LoaiCanHoId", "c.LoaiCanHoId" },
            { "IsDeleted", "c.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("Tang", "t", "t.Id = c.TangId", JoinType.Left, true)
        ]);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                c.Id,
                c.MaCanHo,
                c.TenCanHo,
                c.TangId,
                t.TenTang,
                c.DienTich,
                c.SoPhongNgu,
                c.SoPhongTam,
                c.LoaiCanHoId,
                c.TinhTrangCanHoId
            FROM CanHo c
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<CanHoReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiMap = LoaiCanHo.ToDictionary();
        var tinhTrangMap = TrangThaiCanHo.ToDictionary();

        var items = rows.Select(r => new CanHoDetailResponse
        {
            Id = r.Id,
            MaCanHo = r.MaCanHo,
            TenCanHo = r.TenCanHo,
            TangId = r.TangId,
            TenTang = r.TenTang,
            DienTich = r.DienTich,
            SoPhongNgu = r.SoPhongNgu,
            SoPhongTam = r.SoPhongTam,
            LoaiCanHoId = r.LoaiCanHoId,
            TinhTrangCanHoId = r.TinhTrangCanHoId,
            TenLoaiCanHo = loaiMap.GetValueOrDefault(r.LoaiCanHoId, string.Empty),
            TenTinhTrangCanHo = tinhTrangMap.GetValueOrDefault(r.TinhTrangCanHoId, string.Empty)
        }).ToList();

        return new PagedResult<CanHoDetailResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? items.Count,
                TotalItems = totalCount
            }
        };
    }

    public async Task<CanHoResponse?> GetByIdAsync(GetCanHoByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "c.Id" },
            { "IsDeleted", "c.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("Tang", "t", "t.Id = c.TangId", JoinType.Left, true)
        ]);

        var sql = $"""
            SELECT 
                c.Id, 
                c.TangId, 
                t.TenTang, 
                c.MaCanHo, 
                c.TenCanHo,
                c.DienTich, 
                c.SoPhongNgu, 
                c.SoPhongTam, 
                c.LoaiCanHoId, 
                c.TinhTrangCanHoId
            FROM CanHo c
            {sqlJoins}
            {sqlWhere};
            """;

        var result = await connection.QueryFirstOrDefaultAsync<CanHoDetailReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        if (result is null)
            return null;

        var canHo = new CanHoResponse
        {
            Id = result.Id,
            TangId = result.TangId,
            TenTang = result.TenTang,
            MaCanHo = result.MaCanHo,
            TenCanHo = result.TenCanHo,
            DienTich = result.DienTich,
            SoPhongNgu = result.SoPhongNgu,
            SoPhongTam = result.SoPhongTam,
            LoaiCanHoId = result.LoaiCanHoId,
            TinhTrangCanHoId = result.TinhTrangCanHoId,
            TenLoaiCanHo = LoaiCanHo.ToDictionary().GetValueOrDefault(result.LoaiCanHoId, string.Empty),
            TenTinhTrangCanHo = TrangThaiCanHo.ToDictionary().GetValueOrDefault(result.TinhTrangCanHoId, string.Empty)
        };

        return canHo;
    }
}
