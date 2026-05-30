using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetListTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetTriThucChatbotById;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class TriThucChatbotQueryRepository : ITriThucChatbotQueryRepository
{
    private readonly AppDbContext _dbContext;

    public TriThucChatbotQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TriThucChatbotResponse?> GetByIdAsync(
        GetTriThucChatbotByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id",           "t.Id" },
            { "TieuDe",       "t.TieuDe" },
            { "NoiDung",      "t.NoiDung" },
            { "DanhMuc",      "t.DanhMuc" },
            { "ThuTuHienThi", "t.ThuTuHienThi" },
            { "IsActive",     "t.IsActive" },
            { "IsSynced",     "t.IsSynced" },
            { "IsDeleted",    "t.IsDeleted" },
            { "CreatedAt",    "t.CreatedAt" },
            { "CreatedBy",    "t.CreatedBy" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sql = $"""
            SELECT
                t.Id,
                t.TieuDe,
                t.NoiDung,
                t.DanhMuc,
                t.ThuTuHienThi,
                t.IsActive,
                t.IsSynced,
                t.LastSyncedAt,
                t.CreatedAt,
                t.ModifiedAt,
                t.CreatedBy
            FROM TriThucChatbot t
            {sqlWhere}
            """;

        var row = await connection.QueryFirstOrDefaultAsync<TriThucChatbotReadModel>(
            sql, parameters, transaction: _dbContext.GetDbTransaction());

        return row is null ? null : MapToResponse(row);
    }

    public async Task<PagedResult<TriThucChatbotResponse>> GetListAsync(
        GetListTriThucChatbotSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id",           "t.Id" },
            { "TieuDe",       "t.TieuDe" },
            { "NoiDung",      "t.NoiDung" },
            { "DanhMuc",      "t.DanhMuc" },
            { "ThuTuHienThi", "t.ThuTuHienThi" },
            { "IsActive",     "t.IsActive" },
            { "IsSynced",     "t.IsSynced" },
            { "LastSyncedAt", "t.LastSyncedAt" },
            { "IsDeleted",    "t.IsDeleted" },
            { "CreatedAt",    "t.CreatedAt" },
            { "CreatedBy",    "t.CreatedBy" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere      = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy    = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "ThuTuHienThi");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                t.Id,
                t.TieuDe,
                t.NoiDung,
                t.DanhMuc,
                t.ThuTuHienThi,
                t.IsActive,
                t.IsSynced,
                t.LastSyncedAt,
                t.CreatedAt,
                t.ModifiedAt,
                t.CreatedBy
            FROM TriThucChatbot t
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<TriThucChatbotReadModel>(
            sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        return new PagedResult<TriThucChatbotResponse>
        {
            Items = rows.Select(MapToResponse).ToList(),
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize   = spec.PageSize ?? 20,
                TotalItems = totalCount
            }
        };
    }

    private static TriThucChatbotResponse MapToResponse(TriThucChatbotReadModel r) => new()
    {
        Id           = r.Id,
        TieuDe       = r.TieuDe,
        NoiDung      = r.NoiDung,
        DanhMuc      = r.DanhMuc,
        ThuTuHienThi = r.ThuTuHienThi,
        IsActive     = r.IsActive,
        IsSynced     = r.IsSynced,
        LastSyncedAt = r.LastSyncedAt,
        CreatedAt    = r.CreatedAt,
        UpdatedAt    = r.ModifiedAt,
        CreatedBy    = r.CreatedBy > 0 ? r.CreatedBy.ToString() : null
    };
}
