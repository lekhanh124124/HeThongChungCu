using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.Seeder.DTOs;
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
        int soLuongChuHo,
        int soLuongCuTru,
        int soLuongPhuongTien,
        int soLuongTaiKhoanKhach,
        int soLuongNhanVien,
        YeuCauCounts? soLuongYeuCauCuTru = null,
        YeuCauCounts? soLuongYeuCauPhuongTien = null)
    {
        if (_context.Database.CurrentTransaction != null)
        {
            _logger.LogInformation("Using existing transaction for database seeding...");
            await ExecuteSeedingAsync(soLuongChuHo, soLuongCuTru, soLuongPhuongTien, soLuongTaiKhoanKhach, soLuongNhanVien, soLuongYeuCauCuTru, soLuongYeuCauPhuongTien);
            return;
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Starting new database seeding transaction...");
            await ExecuteSeedingAsync(soLuongChuHo, soLuongCuTru, soLuongPhuongTien, soLuongTaiKhoanKhach, soLuongNhanVien, soLuongYeuCauCuTru, soLuongYeuCauPhuongTien);
            await transaction.CommitAsync();
            _logger.LogInformation("Database Seeding Transaction Committed.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed during database seeding. Rolling back all changes...");
            throw;
        }
    }

    private async Task ExecuteSeedingAsync(
        int soLuongChuHo,
        int soLuongCuTru,
        int soLuongPhuongTien,
        int soLuongTaiKhoanKhach,
        int soLuongNhanVien,
        YeuCauCounts? soLuongYeuCauCuTru,
        YeuCauCounts? soLuongYeuCauPhuongTien)
    {
        // 1. Buildings, Floors, Apartments (Hardcoded)
        await ToaNhaSeeder.SeedAsync(_context, _logger);

        // 2. Admin and Test Accounts
        await UserSeeder.SeedAdminAndTestAccountsAsync(_context, _logger);

        // 3. Staff Members
        await NhanVienSeeder.SeedAsync(_context, _logger, soLuongNhanVien);

        // 4. Residency: ChuHo and CuTru (Includes Users and Accounts)
        await QuanHeCuTruSeeder.SeedAsync(_context, _logger, soLuongChuHo, soLuongCuTru);

        // 5. Guest Accounts (Accounts only)
        await UserSeeder.SeedGuestAccountsAsync(_context, _logger, soLuongTaiKhoanKhach);

        // 6. Vehicles and Cards (Depends on Apartments)
        await PhuongTienSeeder.SeedAsync(_context, _logger, soLuongPhuongTien);

        // 7. Special User Accounts
        await SpecialUserSeeder.SeedGiangKietAsync(_context, _logger);
        await SpecialUserSeeder.SeedHongPhatAsync(_context, _logger);

        // 8. Residency Requests
        await YeuCauCuTruSeeder.SeedAsync(_context, _logger, soLuongYeuCauCuTru);

        // 9. Vehicle Requests
        await YeuCauPhuongTienSeeder.SeedAsync(_context, _logger, soLuongYeuCauPhuongTien);
    }
}
