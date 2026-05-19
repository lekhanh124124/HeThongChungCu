using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLSystem.DTOs;
using HeThongChungCu.Application.Features.QLSystem.Queries.GetBackupHistory;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class TepTaiLieuQueryRepository : ITepTaiLieuQueryRepository
{
    private readonly AppDbContext _dbContext;

    public TepTaiLieuQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<BackupHistoryResponse>> GetBackupHistoryAsync(
        GetBackupHistorySpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "Id" },
            { "FileName", "FileName" },
            { "CreatedAt", "CreatedAt" },
            { "Size", "Size" },
            { "IsDeleted", "IsDeleted" },
            { "LoaiTepId", "LoaiTepId" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, Id, FileName, FileUrl, Size, CreatedAt, ContentType
            FROM TepTaiLieu
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<BackupHistoryReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new BackupHistoryResponse
        {
            FileId = r.Id,
            FileName = r.FileName,
            FileUrl = r.FileUrl,
            Size = r.Size,
            CreatedAt = r.CreatedAt,
            ContentType = r.ContentType
        }).ToList();

        return new PagedResult<BackupHistoryResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? (items.Count == 0 ? 10 : items.Count),
                TotalItems = totalCount
            }
        };
    }
}

public class BackupHistoryReadModel
{
    public int TotalCount { get; set; }
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public long Size { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string ContentType { get; set; } = null!;
}
