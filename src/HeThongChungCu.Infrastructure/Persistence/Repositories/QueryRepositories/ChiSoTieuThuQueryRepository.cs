using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetChiSoById;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class ChiSoTieuThuQueryRepository : IChiSoTieuThuQueryRepository
{
    private readonly AppDbContext _dbContext;

    public ChiSoTieuThuQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ChiSoExcelTemplateDto>> GetExcelTemplateDataAsync(ExportChiSoTemplateSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var parameters = new DynamicParameters();

        var whereMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "IsDeleted", "ch.IsDeleted" },
            { "ToaNhaId", "t.ToaNhaId" },
            { "TangId", "ch.TangId" },
            { "DichVuId", "dv.Id" }
        };

        var toaNhaMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Block", "tn.Block" } };
        var tangMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "TenTang", "t.TenTang" } };
        var dichVuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "TenDichVu", "dv.TenDichVu" } };

        var sqlJoin = DapperQueryBuilder.BuildJoin(spec,
        [
            new("Tang", "t", "ch.TangId = t.Id", Mapping: tangMapping),
            new("ToaNha", "tn", "t.ToaNhaId = tn.Id", Mapping: toaNhaMapping),
            new("DichVu", "dv", "1=1", Type: JoinType.Inner, Mapping: dichVuMapping)
        ], parameters);

        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, whereMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, whereMapping, "MaCanHo");

        var chiSoMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "DichVuId", "DichVuId" },
            { "TrangThaiChiSoId", "TrangThaiChiSoId" }
        };

        var sqlOuterApply = DapperQueryBuilder.BuildOuterApply(
            spec,
            "ChiSoTieuThu",
            "cs",
            "ChiSoMoi",
            "CanHoId = ch.Id AND IsDeleted = 0",
            "Nam DESC, Thang DESC",
            chiSoMapping,
            parameters);

        parameters.Add("Thang", spec.Thang);
        parameters.Add("Nam", spec.Nam);

        // 4. Câu lệnh SQL hoàn chỉnh
        var sql = $@"
            SELECT 
                ch.Id AS CanHoId, 
                ch.MaCanHo, 
                ch.TenCanHo, 
                tn.Block, 
                t.TenTang,
                dv.Id AS DichVuId, 
                dv.TenDichVu,
                ISNULL(cs.ChiSoMoi, 0) AS ChiSoCu,
                ch.MaCanHo + '_' + CAST(dv.Id AS VARCHAR) + '_' + CAST(@Thang AS VARCHAR) + '_' + CAST(@Nam AS VARCHAR) AS MaTraCuu
            FROM CanHo ch
            {sqlJoin}
            {sqlOuterApply}
            {sqlWhere}
            {sqlOrderBy}
        ";

        var transaction = _dbContext.GetDbTransaction();
        var result = await connection.QueryAsync<ChiSoExcelTemplateDto>(sql, parameters, transaction);
        return result.ToList();
    }

    public async Task<PagedResult<ChiSoResponse>> GetListAsync(GetListChiSoSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var parameters = new DynamicParameters();

        // --- Lấy các filter động từ spec ---
        var filterMap = spec.Filters.ToDictionary(f => f.PropertyName, f => f.Value, StringComparer.OrdinalIgnoreCase);

        var whereClauses = new List<string>
        {
            "ch.IsDeleted = 0",
            "dv.IsDeleted = 0"
        };

        if (filterMap.TryGetValue("TangId", out var tangId) && tangId != null)
        {
            whereClauses.Add("ch.TangId = @TangId");
            parameters.Add("TangId", (int)tangId);
        }
        if (filterMap.TryGetValue("ToaNhaId", out var toaNhaId) && toaNhaId != null)
        {
            whereClauses.Add("t.ToaNhaId = @ToaNhaId");
            parameters.Add("ToaNhaId", (int)toaNhaId);
        }
        if (filterMap.TryGetValue("CanHoId", out var canHoId) && canHoId != null)
        {
            whereClauses.Add("ch.Id = @CanHoId");
            parameters.Add("CanHoId", (int)canHoId);
        }
        if (filterMap.TryGetValue("DichVuId", out var dichVuId) && dichVuId != null)
        {
            whereClauses.Add("dv.Id = @DichVuId");
            parameters.Add("DichVuId", (int)dichVuId);
        }

        var sqlWhere = "WHERE " + string.Join(" AND ", whereClauses);

        // --- Tính tháng trước ---
        int thang = filterMap.TryGetValue("Thang", out var thangVal) && thangVal != null ? (int)thangVal : DateTimeOffset.Now.Month;
        int nam = filterMap.TryGetValue("Nam", out var namVal) && namVal != null ? (int)namVal : DateTimeOffset.Now.Year;
        int thangTruoc = thang == 1 ? 12 : thang - 1;
        int namTruoc = thang == 1 ? nam - 1 : nam;

        parameters.Add("Thang", thang);
        parameters.Add("Nam", nam);
        parameters.Add("ThangTruoc", thangTruoc);
        parameters.Add("NamTruoc", namTruoc);

        // --- OrderBy & Paging (dùng DapperQueryBuilder với mapping cố định) ---
        var orderMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "cs_cur.Id" },
            { "MaCanHo", "ch.MaCanHo" },
            { "TenCanHo", "ch.TenCanHo" },
            { "TenDichVu", "dv.TenDichVu" },
            { "Thang", "cs_cur.Thang" },
            { "Nam", "cs_cur.Nam" },
            { "NgayGhiNhan", "cs_cur.NgayGhiNhan" },
            { "TrangThaiChiSoId", "cs_cur.TrangThaiChiSoId" }
        };
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, orderMapping, "MaCanHo");
        var sqlPaging = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $@"
            SELECT
                COUNT(*) OVER()                                                     AS TotalCount,
                ch.Id                                                               AS CanHoId,
                ch.MaCanHo,
                ch.TenCanHo,
                dv.Id                                                               AS DichVuId,
                dv.TenDichVu,
                ISNULL(cs_cur.Id, 0)                                                AS Id,
                ISNULL(cs_cur.ChiSoCu, ISNULL(cs_prev.ChiSoMoi, 0))               AS ChiSoCu,
                ISNULL(cs_cur.ChiSoMoi, 0)                                         AS ChiSoMoi,
                cs_cur.Thang,
                cs_cur.Nam,
                cs_cur.NgayGhiNhan,
                ISNULL(cs_cur.TrangThaiChiSoId, 0)                                 AS TrangThaiChiSoId,
                cs_cur.MaTraCuu
            FROM CanHo ch
            INNER JOIN Tang t   ON ch.TangId = t.Id
            INNER JOIN ToaNha tn ON t.ToaNhaId = tn.Id
            CROSS JOIN DichVu dv
            OUTER APPLY (
                SELECT TOP 1
                    Id, ChiSoCu, ChiSoMoi, Thang, Nam, NgayGhiNhan, TrangThaiChiSoId, MaTraCuu
                FROM ChiSoTieuThu
                WHERE CanHoId = ch.Id
                  AND DichVuId = dv.Id
                  AND Thang = @Thang
                  AND Nam = @Nam
                  AND IsDeleted = 0
            ) cs_cur
            OUTER APPLY (
                SELECT TOP 1 ChiSoMoi
                FROM ChiSoTieuThu
                WHERE CanHoId = ch.Id
                  AND DichVuId = dv.Id
                  AND Thang = @ThangTruoc
                  AND Nam = @NamTruoc
                  AND IsDeleted = 0
            ) cs_prev
            {sqlWhere}
            {sqlOrderBy}
            {sqlPaging};
        ";

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<dynamic>(sql, parameters, transaction)).ToList();

        if (rows.Count == 0)
        {
            return new PagedResult<ChiSoResponse>
            {
                Items = new List<ChiSoResponse>(),
                PagingInfo = new PagingInfo
                {
                    PageNumber = spec.PageNumber,
                    PageSize = spec.PageSize,
                    TotalItems = 0
                }
            };
        }

        var items = rows.Select(row =>
        {
            int trangThaiId = (int)row.TrangThaiChiSoId;
            decimal chiSoCu = (decimal)row.ChiSoCu;
            decimal chiSoMoi = (decimal)row.ChiSoMoi;

            return new ChiSoResponse
            {
                Id = (int)row.Id,
                CanHoId = (int)row.CanHoId,
                MaCanHo = row.MaCanHo,
                TenCanHo = row.TenCanHo,
                DichVuId = (int)row.DichVuId,
                TenDichVu = row.TenDichVu,
                ChiSoCu = chiSoCu,
                ChiSoMoi = chiSoMoi,
                SoLuong = chiSoMoi - chiSoCu,
                Thang = row.Thang != null ? (int)row.Thang : thang,
                Nam = row.Nam != null ? (int)row.Nam : nam,
                NgayGhiNhan = row.NgayGhiNhan != null ? (DateTimeOffset?)row.NgayGhiNhan : null,
                TrangThaiChiSoId = trangThaiId,
                TrangThaiChiSoTen = trangThaiId > 0
                    ? TrangThaiChiSo.FromValue(trangThaiId)!.Name
                    : string.Empty,
                MaTraCuu = row.MaTraCuu
            };
        }).ToList();

        var totalCount = (int)rows.First().TotalCount;

        return new PagedResult<ChiSoResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber,
                PageSize = spec.PageSize,
                TotalItems = totalCount
            }
        };
    }

    public async Task<ChiSoDetailResponse?> GetByIdAsync(GetChiSoByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "cs.Id" },
            { "IsDeleted", "cs.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, mapping, parameters);
        var sqlJoin = DapperQueryBuilder.BuildJoin(spec, new[]
        {
            new JoinDefinition("CanHo", "ch", "cs.CanHoId = ch.Id", JoinType.Inner),
            new JoinDefinition("DichVu", "dv", "cs.DichVuId = dv.Id", JoinType.Inner),
            new JoinDefinition("TepTaiLieu", "tl", "cs.AnhDongHoId = tl.Id", JoinType.Left)
        }, parameters);

        var sql = $@"
            SELECT 
                cs.Id, cs.CanHoId, ch.MaCanHo, ch.TenCanHo,
                cs.DichVuId, dv.TenDichVu,
                cs.ChiSoCu, cs.ChiSoMoi, cs.Thang, cs.Nam, cs.NgayGhiNhan,
                cs.TrangThaiChiSoId, cs.GhiChu, cs.HoaDonId, cs.AnhDongHoId,
                tl.FileUrl AS AnhDongHoUrl, cs.MaTraCuu
            FROM ChiSoTieuThu cs
            {sqlJoin}
            {sqlWhere}";

        var transaction = _dbContext.GetDbTransaction();
        var row = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, parameters, transaction);

        if (row == null) return null;

        return new ChiSoDetailResponse
        {
            Id = row.Id,
            CanHoId = row.CanHoId,
            MaCanHo = row.MaCanHo,
            TenCanHo = row.TenCanHo,
            DichVuId = row.DichVuId,
            TenDichVu = row.TenDichVu,
            ChiSoCu = row.ChiSoCu,
            ChiSoMoi = row.ChiSoMoi,
            SoLuong = row.ChiSoMoi - row.ChiSoCu,
            Thang = row.Thang,
            Nam = row.Nam,
            NgayGhiNhan = row.NgayGhiNhan,
            TrangThaiChiSoId = (int)row.TrangThaiChiSoId,
            TrangThaiChiSoTen = TrangThaiChiSo.FromValue((int)row.TrangThaiChiSoId)!.Name,
            MaTraCuu = row.MaTraCuu,
            GhiChu = row.GhiChu,
            AnhDongHoId = row.AnhDongHoId,
            AnhDongHoUrl = row.AnhDongHoUrl,
            HoaDonId = row.HoaDonId
        };
    }
}
