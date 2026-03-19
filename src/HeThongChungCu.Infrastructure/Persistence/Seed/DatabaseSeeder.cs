using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(
        int soLuongNguoiDung, 
        int soLuongToaNha, 
        int soLuongTangMoiToa, 
        int soLuongCanHoMoiTang, 
        int soLuongPhuongTien,
        int soLuongCuTru,
        int soLuongChiSoTieuThuMoiCanHo,
        int soLuongThePhuongTien,
        int soLuongTangHamMoiToa)
    {
        await UserSeeder.SeedAsync(_context, _logger, soLuongNguoiDung);
        await ToaNhaSeeder.SeedAsync(_context, _logger, soLuongToaNha, soLuongTangMoiToa, soLuongTangHamMoiToa);
        await CanHoSeeder.SeedAsync(_context, _logger, soLuongCanHoMoiTang);
        await QuanHeCuTruSeeder.SeedAsync(_context, _logger, soLuongCuTru);
        await PhuongTienSeeder.SeedAsync(_context, _logger, soLuongPhuongTien, soLuongThePhuongTien);
        await ChiSoTieuThuSeeder.SeedAsync(_context, _logger, soLuongChiSoTieuThuMoiCanHo);
    }
}
