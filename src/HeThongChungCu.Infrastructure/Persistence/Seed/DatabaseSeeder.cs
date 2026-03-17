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

    public async Task SeedDatabaseAsync(int numberOfUsers, int numberOfBuildings, int numberOfFloorsPerBuilding, int numberOfApartmentsPerFloor, int numberOfVehicles)
    {
        await UserSeeder.SeedAsync(_context, _logger, numberOfUsers);
        await ToaNhaSeeder.SeedAsync(_context, _logger, numberOfBuildings, numberOfFloorsPerBuilding);
        await CanHoSeeder.SeedAsync(_context, _logger, numberOfApartmentsPerFloor);
        await QuanHeCuTruSeeder.SeedAsync(_context, _logger);
        await PhuongTienSeeder.SeedAsync(_context, _logger, numberOfVehicles);
        await ChiSoTieuThuSeeder.SeedAsync(_context, _logger);
    }
}
