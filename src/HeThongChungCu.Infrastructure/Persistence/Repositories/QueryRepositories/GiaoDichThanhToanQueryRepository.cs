using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class GiaoDichThanhToanQueryRepository : IGiaoDichThanhToanQueryRepository
{
    private readonly AppDbContext _dbContext;

    public GiaoDichThanhToanQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GiaoDichThanhToanResponse>> GetByHoaDonIdAsync(int hoaDonId, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("HoaDonId", hoaDonId);

        var sql = """
            SELECT gd.Id, ct.HoaDonId, gd.ChiTietHoaDonId, gd.SoTien, gd.PhuongThucThanhToanId, gd.NgayGiaoDich, gd.MaGiaoDich, gd.GhiChu,
                   ct.TenMucPhi
            FROM GiaoDichThanhToan gd
            INNER JOIN ChiTietHoaDon ct ON ct.Id = gd.ChiTietHoaDonId
            WHERE ct.HoaDonId = @HoaDonId
            ORDER BY gd.NgayGiaoDich DESC, gd.Id DESC;
            """;

        var rows = (await connection.QueryAsync<FlatGiaoDichRow>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();

        var result = rows
            .GroupBy(x => new { x.MaGiaoDich, x.NgayGiaoDich, x.PhuongThucThanhToanId, x.GhiChu })
            .Select(g => new GiaoDichThanhToanResponse
            {
                Id = g.First().Id,
                HoaDonId = g.First().HoaDonId,
                SoTien = g.Sum(x => x.SoTien),
                PhuongThucThanhToanId = g.Key.PhuongThucThanhToanId,
                NgayGiaoDich = g.Key.NgayGiaoDich,
                MaGiaoDich = g.Key.MaGiaoDich,
                GhiChu = g.Key.GhiChu,
                ChiTiet = g.Select(x => new GiaoDichThanhToanChiTietResponse
                {
                    ChiTietHoaDonId = x.ChiTietHoaDonId,
                    TenMucPhi = x.TenMucPhi,
                    SoTienPhanBo = x.SoTien
                }).ToList()
            })
            .OrderByDescending(x => x.NgayGiaoDich)
            .ToList();

        return result;
    }

    private sealed class FlatGiaoDichRow
    {
        public int Id { get; set; }
        public int HoaDonId { get; set; }
        public int ChiTietHoaDonId { get; set; }
        public decimal SoTien { get; set; }
        public int PhuongThucThanhToanId { get; set; }
        public DateTimeOffset NgayGiaoDich { get; set; }
        public string? MaGiaoDich { get; set; }
        public string? GhiChu { get; set; }
        public string TenMucPhi { get; set; } = null!;
    }
}
