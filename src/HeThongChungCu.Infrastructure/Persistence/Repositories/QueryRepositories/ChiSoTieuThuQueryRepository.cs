using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;
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

        // 1. Định nghĩa Mapping cho từng bảng để BuildJoin/BuildWhere có thể ánh xạ đúng
        var canHoMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "IsDeleted", "ch.IsDeleted" } };
        var toaNhaMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "ToaNhaId", "tn.Id" }, { "Block", "tn.Block" } };
        var tangMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "TangId", "t.Id" }, { "TenTang", "t.TenTang" } };
        var dichVuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "DichVuId", "dv.Id" } };

        // 2. Sử dụng Helper để build phần JOIN
        // BuildJoin sẽ tự động bóc tách các filter từ Spec (DichVuId, ToaNhaId, TangId) nếu thấy PropertyName khớp trong Mapping
        var sqlJoin = DapperQueryBuilder.BuildJoin(spec,
        [
            new("Tang", "t", "ch.TangId = t.Id", Mapping: tangMapping),
            new("ToaNha", "tn", "t.ToaNhaId = tn.Id", Mapping: toaNhaMapping),
            new("DichVu", "dv", "1=1", Type: JoinType.Inner, Mapping: dichVuMapping)
        ], parameters);

        // 3. Build phần WHERE và ORDER BY cho câu truy vấn chính (bảng CanHo)
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, canHoMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, canHoMapping, "MaCanHo");

        // 4. Câu lệnh SQL hoàn chỉnh
        // GIẢI THÍCH:
        // 4. Build OUTER APPLY để lấy "Số cũ" (Chỉ số mới nhất của kỳ trước)
        // Helper này sẽ tự động đưa các filter từ Spec (DichVuId, TrangThaiChiSoId) vào trong subquery
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

        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "cs.Id" },
            { "Thang", "cs.Thang" },
            { "Nam", "cs.Nam" },
            { "TrangThaiChiSoId", "cs.TrangThaiChiSoId" },
            { "MaCanHo", "ch.MaCanHo" },
            { "TenCanHo", "ch.TenCanHo" },
            { "TenDichVu", "dv.TenDichVu" },
            { "NgayGhiNhan", "cs.NgayGhiNhan" },
            { "IsDeleted", "cs.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, mapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, mapping, "Id DESC");
        var sqlPaging = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sqlJoin = DapperQueryBuilder.BuildJoin(spec, new[]
        {
            new JoinDefinition("CanHo", "ch", "cs.CanHoId = ch.Id"),
            new JoinDefinition("DichVu", "dv", "cs.DichVuId = dv.Id")
        }, parameters);

        var sql = $@"
            SELECT COUNT(*) OVER() AS TotalCount,
                cs.Id, cs.CanHoId, ch.MaCanHo, ch.TenCanHo,
                cs.DichVuId, dv.TenDichVu,
                cs.ChiSoCu, cs.ChiSoMoi, cs.Thang, cs.Nam, cs.NgayGhiNhan,
                cs.TrangThaiChiSoId, cs.MaTraCuu
            FROM ChiSoTieuThu cs
            {sqlJoin}
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

        var items = rows.Select(row => new ChiSoResponse
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
            TrangThaiChiSoId = Domain.Enums.TrangThaiChiSo.FromValue((int)row.TrangThaiChiSoId),
            MaTraCuu = row.MaTraCuu
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

    public async Task<ChiSoDetailResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var sql = @"
            SELECT 
                cs.Id, cs.CanHoId, ch.MaCanHo, ch.TenCanHo,
                cs.DichVuId, dv.TenDichVu,
                cs.ChiSoCu, cs.ChiSoMoi, cs.Thang, cs.Nam, cs.NgayGhiNhan,
                cs.TrangThaiChiSoId, cs.GhiChu, cs.HoaDonId, cs.AnhDongHoId,
                tl.Url AS AnhDongHoUrl, cs.MaTraCuu
            FROM ChiSoTieuThu cs
            JOIN CanHo ch ON cs.CanHoId = ch.Id
            JOIN DichVu dv ON cs.DichVuId = dv.Id
            LEFT JOIN TepTaiLieu tl ON cs.AnhDongHoId = tl.Id
            WHERE cs.Id = @Id AND cs.IsDeleted = 0";

        var transaction = _dbContext.GetDbTransaction();
        var row = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id }, transaction);

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
            TrangThaiChiSoId = Domain.Enums.TrangThaiChiSo.FromValue((int)row.TrangThaiChiSoId),
            MaTraCuu = row.MaTraCuu,
            GhiChu = row.GhiChu,
            AnhDongHoId = row.AnhDongHoId,
            AnhDongHoUrl = row.AnhDongHoUrl,
            HoaDonId = row.HoaDonId
        };
    }
}
