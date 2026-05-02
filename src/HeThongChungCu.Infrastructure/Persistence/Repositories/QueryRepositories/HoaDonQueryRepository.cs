using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;
using Microsoft.EntityFrameworkCore.Storage;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class HoaDonQueryRepository : IHoaDonQueryRepository
{
    private readonly AppDbContext _dbContext;

    public HoaDonQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<HoaDonResponse>> GetListAsync(
        GetListHoaDonSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "hd.Id" },
            { "MaHoaDon", "hd.MaHoaDon" },
            { "CanHoId", "hd.CanHoId" },
            { "DotThanhToanId", "hd.DotThanhToanId" },
            { "Thang", "hd.Thang" },
            { "Nam", "hd.Nam" },
            { "TrangThaiHoaDonId", "hd.TrangThaiHoaDonId" },
            { "NgayLap", "hd.NgayLap" },
            { "NgayHanThanhToan", "hd.NgayHanThanhToan" },
            { "TongTien", "hd.TongTien" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   hd.Id, hd.CanHoId, hd.DotThanhToanId, hd.MaHoaDon, hd.Thang, hd.Nam, 
                   hd.NgayLap, hd.NgayHanThanhToan, hd.TongTien, hd.TrangThaiHoaDonId
            FROM HoaDon hd
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<HoaDonReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new HoaDonResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            MaHoaDon = r.MaHoaDon,
            Thang = r.Thang,
            Nam = r.Nam,
            NgayLap = r.NgayLap,
            NgayHanThanhToan = r.NgayHanThanhToan,
            TongTien = r.TongTien,
            TrangThaiHoaDonId = r.TrangThaiHoaDonId,
            TrangThaiHoaDonTen = TrangThaiHoaDon.FromValue(r.TrangThaiHoaDonId)?.Name ?? string.Empty
        }).ToList();

        return new PagedResult<HoaDonResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? (items.Count > 0 ? items.Count : 10),
                TotalItems = totalCount
            }
        };
    }

    public async Task<HoaDonDetailResponse?> GetByIdAsync(
        GetHoaDonByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "hd.Id" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoinsCt = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("ChiTietHoaDon", "ct", "ct.HoaDonId = hd.Id", Type: JoinType.Inner)
        ], parameters);

        var sql = $"""
            SELECT hd.Id, hd.CanHoId, hd.DotThanhToanId, hd.MaHoaDon, hd.Thang, hd.Nam, 
                   hd.NgayLap, hd.NgayHanThanhToan, hd.TongTien, hd.TrangThaiHoaDonId, hd.GhiChu
            FROM HoaDon hd
            {sqlWhere};

            SELECT ct.Id, ct.HoaDonId, ct.LoaiChiTietHoaDonId, ct.TenMucPhi, 
                   ct.SoLuong, ct.DonGia, ct.ThanhTien, ct.GhiChu
            FROM HoaDon hd
            {sqlJoinsCt}
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());

        var hoaDonRow = await multi.ReadFirstOrDefaultAsync<HoaDonReadModel>();
        if (hoaDonRow == null) return null;

        var chiTietRows = (await multi.ReadAsync<ChiTietHoaDonReadModel>()).ToList();

        var response = new HoaDonDetailResponse
        {
            Id = hoaDonRow.Id,
            CanHoId = hoaDonRow.CanHoId,
            MaHoaDon = hoaDonRow.MaHoaDon,
            Thang = hoaDonRow.Thang,
            Nam = hoaDonRow.Nam,
            NgayLap = hoaDonRow.NgayLap,
            NgayHanThanhToan = hoaDonRow.NgayHanThanhToan,
            TongTien = hoaDonRow.TongTien,
            TrangThaiHoaDonId = hoaDonRow.TrangThaiHoaDonId,
            TrangThaiHoaDonTen = TrangThaiHoaDon.FromValue(hoaDonRow.TrangThaiHoaDonId)?.Name ?? string.Empty,
            GhiChu = hoaDonRow.GhiChu,
            ChiTietHoaDons = chiTietRows.Select(ct => new ChiTietHoaDonResponse
            {
                Id = ct.Id,
                LoaiChiTietHoaDonId = ct.LoaiChiTietHoaDonId,
                LoaiChiTietHoaDonTen = LoaiChiTietHoaDon.FromValue(ct.LoaiChiTietHoaDonId)?.Name ?? string.Empty,
                TenMucPhi = ct.TenMucPhi,
                SoLuong = ct.SoLuong,
                DonGia = ct.DonGia,
                ThanhTien = ct.ThanhTien,
                GhiChu = ct.GhiChu
            }).ToList()
        };

        return response;
    }
}
