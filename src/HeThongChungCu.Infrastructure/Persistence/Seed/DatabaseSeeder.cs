using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.Seeder.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

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
        await _semaphore.WaitAsync();
        try
        {
            await ExecuteSeedingAsync(
                soLuongChuHo,
                soLuongCuTru,
                soLuongPhuongTien,
                soLuongTaiKhoanKhach,
                soLuongNhanVien,
                soLuongYeuCauCuTru,
                soLuongYeuCauPhuongTien);
        }
        finally
        {
            _semaphore.Release();
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
        var hasExistingTransaction = _context.Database.CurrentTransaction != null;
        var transaction = hasExistingTransaction ? null : await _context.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Cleaning up existing database records...");
            await CleanupAsync();

            // IMPORTANT: Clear the ChangeTracker to ensure no "Deleted" entities 
            // are still being tracked by EF Core, which could cause conflicts during seeding.
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Initializing seeders and uniqueness trackers...");
            await UserSeeder.InitializeAsync(_context);
            await PhuongTienSeeder.InitializeAsync(_context);

            // Pre-register hardcoded special users to prevent random generator collisions
            UserSeeder.RegisterSpecialValues();

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

            // Final SaveChanges if anything was missed or to commit changes from seeders that don't save.
            // Most of our optimized seeders now save at their own end.
            await _context.SaveChangesAsync();

            if (transaction != null)
            {
                await transaction.CommitAsync();
                _logger.LogInformation("Database Seeding Transaction Committed.");
            }
        }
        catch (Exception ex)
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Transaction failed during database seeding. Rolling back all changes...");
            }
            else
            {
                _logger.LogError(ex, "Error occurred during database seeding in an existing transaction.");
            }
            throw;
        }
        finally
        {
            if (transaction != null) await transaction.DisposeAsync();
        }
    }

    private async Task CleanupAsync()
    {
        _logger.LogInformation("Cleaning up existing database records...");

        // Order is critical due to foreign key constraints. 
        // We delete children before parents.
        var tables = new[]
        {
            "ThePhuongTien", 
            "PhuongTien",
            "YeuCau", // TPH for YeuCauCuTru, YeuCauPhuongTien
            "QuanHeCuTru",
            "TepTaiLieu",
            "TaiLieu", // TPH for TaiLieuNguoiDung, YeuCauTaiLieuCuTru
            "CanHo", 
            "Tang", 
            "ToaNha",
            "PhanBoThongBao", 
            "ThongBao",
            "DangKyDichVu", 
            "ChiSoTieuThu", 
            "HoaDonDoiTac", 
            "BangGia", 
            "DichVu", 
            "DoiTac",
            "Token", // singular from TokensConfiguration
            "TaiKhoan", 
            "NhanVien", 
            "PhanQuyen",
            "NguoiDung"
        };

        foreach (var table in tables)
        {
            try
            {
                // Use a direct delete command. Truncate might fail due to FKs even if empty.
                int deletedRows = await _context.Database.ExecuteSqlRawAsync($"DELETE FROM [{table}]");
                if (deletedRows > 0)
                {
                    _logger.LogInformation($"Cleared {deletedRows} rows from table {table}.");
                }
            }
            catch (Exception ex)
            {
                // Some tables might not exist or have complex circular refs handled by other deletes
                _logger.LogWarning($"Table {table} cleanup note: {ex.Message}");
            }
        }

        _logger.LogInformation("Cleanup completed.");
    }
}
