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

    public static void ClearAllDomainEvents(AppDbContext context)
    {
        var entities = context.ChangeTracker
            .Entries<HeThongChungCu.Domain.Common.AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }
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

            _context.ChangeTracker.Clear();

            _logger.LogInformation("Initializing seeders and synchronization...");
            await UserSeeder.ResetAndSyncAsync(_context);
            await PhuongTienSeeder.ResetAndSyncAsync(_context);

            // Seed Cứng (Fixed Seeds)
            _logger.LogInformation("Seeding fixed data...");
            await SpecialUserSeeder.SeedAdminAndTestAccountsAsync(_context, _logger);
            await ToaNhaSeeder.SeedAsync(_context, _logger);
            await DichVuSeeder.SeedAsync(_context, _logger);

            // Lấy ID Admin sau khi đã seed ở trên
            var admin = await _context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
            var adminId = admin?.Id ?? 0;

            await SpecialUserSeeder.SeedGiangKietAsync(_context, _logger);
            await SpecialUserSeeder.SeedHongPhatAsync(_context, _logger);

            // Seed Ngẫu Nhiên (Random Seeds)
            _logger.LogInformation("Seeding random/variable data...");
            await NhanVienSeeder.SeedAsync(_context, _logger, soLuongNhanVien);
            await QuanHeCuTruSeeder.SeedAsync(_context, _logger, soLuongChuHo, soLuongCuTru);
            await UserSeeder.SeedGuestAccountsAsync(_context, _logger, soLuongTaiKhoanKhach, adminId);
            await PhuongTienSeeder.SeedAsync(_context, _logger, soLuongPhuongTien);
            await YeuCauCuTruSeeder.SeedAsync(_context, _logger, soLuongYeuCauCuTru);
            await YeuCauPhuongTienSeeder.SeedAsync(_context, _logger, soLuongYeuCauPhuongTien);
            await DangKyDichVuSeeder.SeedAsync(_context, _logger);

            ClearAllDomainEvents(_context);
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
        _logger.LogInformation("Performing dynamic cleanup of all database tables...");

        var tableNames = _context.Model.GetEntityTypes()
            .Select(t => t.GetSchema() == null ? $"[{t.GetTableName()}]" : $"[{t.GetSchema()}].[{t.GetTableName()}]")
            .Distinct()
            .ToList();

        try
        {
            // 1. Disable all foreign key constraints
            foreach (var table in tableNames)
            {
                await _context.Database.ExecuteSqlAsync($"ALTER TABLE {table} NOCHECK CONSTRAINT ALL");
            }

            // 2. Delete data from each table
            foreach (var table in tableNames)
            {
                try
                {
                    int deletedRows = await _context.Database.ExecuteSqlAsync($"DELETE FROM {table}");
                    if (deletedRows > 0)
                    {
                        _logger.LogInformation($"Cleared {deletedRows} rows from table {table}.");

                        // Reseed identity columns so IDs start from 1
                        try
                        {
                            await _context.Database.ExecuteSqlAsync($"DBCC CHECKIDENT ('{table}', RESEED, 0)");
                        }
                        catch
                        {
                            // Table might not have an identity column
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Could not clear table {table}: {ex.Message}");
                }
            }

            // 3. Re-enable all foreign key constraints
            foreach (var table in tableNames)
            {
                await _context.Database.ExecuteSqlAsync($"ALTER TABLE {table} WITH CHECK CHECK CONSTRAINT ALL");
            }

            _logger.LogInformation("Database cleanup completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the deep cleanup process.");
        }
    }
}
