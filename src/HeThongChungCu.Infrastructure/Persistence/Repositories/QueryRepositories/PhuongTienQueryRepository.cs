using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSPhuongTienTrongChungCu;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

internal sealed class PhuongTienQueryRepository : IPhuongTienQueryRepository
{
    private readonly AppDbContext _dbContext;

    public PhuongTienQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<PhuongTienResponse>> LayDSPhuongTienTrongChungCu(
        LayDSPhuongTienTrongChungCuSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "p.Id" },
            { "CanHoId", "p.CanHoId" },
            { "MaCanHo", "ch.MaCanHo" },
            { "MaTang", "tg.MaTang" },
            { "MaToaNha", "tn.MaToaNha" },
            { "TenPhuongTien", "p.TenPhuongTien" },
            { "LoaiPhuongTienId", "p.LoaiPhuongTienId" },
            { "BienSo", "p.BienSo" },
            { "MauXe", "p.MauXe" },
            { "TrangThaiPhuongTienId", "p.TrangThaiPhuongTienId" },
            { "ToaNhaId", "tg.ToaNhaId" },
            { "TangId", "ch.TangId" },
            { "IsDeleted", "p.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("CanHo", "ch", "ch.Id = p.CanHoId"),
            new JoinDefinition("Tang", "tg", "tg.Id = ch.TangId"),
            new JoinDefinition("ToaNha", "tn", "tn.Id = tg.ToaNhaId")
        ]);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                p.Id,
                ch.Id AS CanHoId,
                ch.MaCanHo,
                tg.MaTang,
                tn.MaToaNha,
                p.TenPhuongTien,
                p.LoaiPhuongTienId,
                p.BienSo,
                p.MauXe,
                p.TrangThaiPhuongTienId
            FROM PhuongTien p
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<PhuongTienReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiPhuongTienMap = LoaiPhuongTien.ToDictionary();
        var trangThaiPhuongTienMap = TrangThaiPhuongTien.ToDictionary();

        var items = rows.Select(r => new PhuongTienResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            MaToaNha = r.MaToaNha,
            MaTang = r.MaTang,
            MaCanHo = r.MaCanHo,
            TenPhuongTien = r.TenPhuongTien,
            LoaiPhuongTienId = r.LoaiPhuongTienId,
            TenLoaiPhuongTien = loaiPhuongTienMap.GetValueOrDefault(r.LoaiPhuongTienId, string.Empty),
            BienSo = r.BienSo,
            MauXe = r.MauXe,
            TrangThaiPhuongTienId = r.TrangThaiPhuongTienId,
            TenTrangThaiPhuongTien = trangThaiPhuongTienMap.GetValueOrDefault(r.TrangThaiPhuongTienId, string.Empty)
        }).ToList();

        return new PagedResult<PhuongTienResponse>
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

    public async Task<PhuongTienResponse?> GetByIdAsync(GetPhuongTienByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "p.Id" },
            { "IsDeleted", "p.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("CanHo", "ch", "ch.Id = p.CanHoId"),
            new JoinDefinition("Tang", "tg", "tg.Id = ch.TangId"),
            new JoinDefinition("ToaNha", "tn", "tn.Id = tg.ToaNhaId")
        ]);

        var sqlJoinsThe = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("ThePhuongTien", "tpt", "tpt.PhuongTienId = p.Id", JoinType.Left, true)
        ]);

        var sqlJoinsTtl = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("TepTaiLieu", "ttl", "ttl.PhuongTienId = p.Id", JoinType.Left, true, Discriminators: [("LoaiTepTaiLieu", "TepPhuongTien")])
        ]);

        var sql = $"""
            -- 1. Main Info
            SELECT
                p.Id,
                ch.Id AS CanHoId,
                ch.MaCanHo,
                tg.MaTang,
                tn.MaToaNha,
                p.TenPhuongTien,
                p.LoaiPhuongTienId,
                p.BienSo,
                p.MauXe,
                p.TrangThaiPhuongTienId
            FROM PhuongTien p
            {sqlJoins}
            {sqlWhere};

            -- 2. Detail Info (1-N)
            SELECT
                tpt.Id,
                tpt.PhuongTienId,
                tpt.MaThe,
                tpt.NgayBatDau,
                tpt.NgayKetThuc,
                tpt.TrangThaiId AS TrangThaiThePhuongTienId
            FROM PhuongTien p
            {sqlJoinsThe}
            {sqlWhere};

            SELECT
                ttl.Id AS FileId,
                ttl.FileName,
                ttl.FileUrl,
                ttl.ContentType
            FROM PhuongTien p
            {sqlJoinsTtl}
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());
        var phuongTien = await multi.ReadFirstOrDefaultAsync<PhuongTienReadModel>();

        if (phuongTien == null)
            return null;

        var cards = (await multi.ReadAsync<ThePhuongTienReadModel>()).ToList();
        var imageRows = (await multi.ReadAsync<PhuongTienFileReadModel>()).ToList();

        return new PhuongTienResponse
        {
            Id = phuongTien.Id,
            CanHoId = phuongTien.CanHoId,
            MaToaNha = phuongTien.MaToaNha,
            MaTang = phuongTien.MaTang,
            MaCanHo = phuongTien.MaCanHo,
            TenPhuongTien = phuongTien.TenPhuongTien,
            LoaiPhuongTienId = phuongTien.LoaiPhuongTienId,
            TenLoaiPhuongTien = LoaiPhuongTien.FromValue(phuongTien.LoaiPhuongTienId)?.Name ?? string.Empty,
            BienSo = phuongTien.BienSo,
            MauXe = phuongTien.MauXe,
            TrangThaiPhuongTienId = phuongTien.TrangThaiPhuongTienId,
            TenTrangThaiPhuongTien = TrangThaiPhuongTien.FromValue(phuongTien.TrangThaiPhuongTienId)?.Name ?? string.Empty,
            ThePhuongTiens = cards.Select(c => new ThePhuongTienResponse
            {
                Id = c.Id,
                PhuongTienId = c.PhuongTienId,
                MaThe = c.MaThe,
                NgayBatDau = c.NgayBatDau,
                NgayKetThuc = c.NgayKetThuc,
                TrangThaiThePhuongTienId = c.TrangThaiThePhuongTienId,
                TenTrangThaiThePhuongTien = TrangThaiThePhuongTien.FromValue(c.TrangThaiThePhuongTienId)?.Name ?? string.Empty
            }).ToList(),
            HinhAnhPhuongTiens = imageRows.Select(f => new UploadFileResponse(f.FileId, f.FileName, f.FileUrl, f.ContentType)).ToList()
        };
    }
}
