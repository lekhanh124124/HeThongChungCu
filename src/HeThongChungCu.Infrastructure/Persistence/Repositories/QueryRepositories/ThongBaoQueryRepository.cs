using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;
using HeThongChungCu.Domain.Enums;
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

    public async Task<PagedResult<ThongBaoResponse>> GetDSThongBaoAsync(int userId, int pageNumber, int pageSize, bool? onlyUnread, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var sqlWhere = " WHERE pb.NguoiDungId = @UserId ";
        if (onlyUnread == true)
        {
            sqlWhere += " AND pb.IsRead = 0 ";
        }

        var offset = (pageNumber - 1) * pageSize;

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
            JOIN ThongBao t ON pb.ThongBaoId = t.Id
            {sqlWhere}
            ORDER BY t.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var parameters = new { UserId = userId, Offset = offset, PageSize = pageSize };
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
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount
            }
        };
    }
}
