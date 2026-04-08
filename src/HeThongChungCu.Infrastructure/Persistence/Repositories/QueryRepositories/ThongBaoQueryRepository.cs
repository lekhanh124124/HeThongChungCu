using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ThongBao.DTOs;
using HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence;
using System.Data;

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
            { "IsDeleted", "t.IsDeleted" }
        };

        var joins = new[]
        {
            new JoinDefinition("ThongBao", "t", "pb.ThongBaoId = t.Id", Type: JoinType.Inner)
        };

        var sqlJoins = DapperQueryBuilder.BuildJoin(joins);
        var (sqlWhere, parameters) = DapperQueryBuilder.BuildWhere(spec, columnMapping);
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

        var rows = (await connection.QueryAsync<dynamic>(sql, parameters)).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;
        var loaiThongBaoMap = LoaiThongBao.ToDictionary();

        var items = rows.Select(r => new ThongBaoResponse
        {
            Id = (int)r.Id,
            ThongBaoId = (int)r.ThongBaoId,
            TieuDe = (string)r.TieuDe,
            NoiDung = (string)r.NoiDung,
            LoaiThongBaoId = (int)r.LoaiThongBaoId,
            TenLoaiThongBao = loaiThongBaoMap.GetValueOrDefault((int)r.LoaiThongBaoId, string.Empty),
            ReferenceId = (string?)r.ReferenceId,
            Metadata = (string?)r.Metadata,
            IsRead = (bool)r.IsRead,
            CreatedAt = (DateTimeOffset)r.CreatedAt,
            ReadAt = (DateTimeOffset?)r.ReadAt
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
