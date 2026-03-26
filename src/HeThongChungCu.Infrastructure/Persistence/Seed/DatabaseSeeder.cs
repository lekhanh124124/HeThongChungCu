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
        int soLuongChuHo,
        int soLuongCuTru,
        int soLuongPhuongTien,
        int soLuongTaiKhoanKhach)
    {
        // 1. Buildings, Floors, Apartments (Hardcoded)
        await ToaNhaSeeder.SeedAsync(_context, _logger);
        
        // 2. Admin and Test Accounts
        await UserSeeder.SeedAdminAndTestAccountsAsync(_context, _logger);

        // 3. Residency: ChuHo and CuTru (Includes Users and Accounts)
        await QuanHeCuTruSeeder.SeedAsync(_context, _logger, soLuongChuHo, soLuongCuTru);

        // 3. Guest Accounts (Accounts only)
        await UserSeeder.SeedGuestAccountsAsync(_context, _logger, soLuongTaiKhoanKhach);
        
        // 4. Vehicles and Cards (Depends on Apartments)
        await PhuongTienSeeder.SeedAsync(_context, _logger, soLuongPhuongTien);

        // 5. Special User Account (Giang Tuấn Kiệt)
        await SpecialUserSeeder.SeedGiangKietAsync(_context, _logger);
    }
}
