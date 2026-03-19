using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Application.Features.Seeder.Commands.SeedDatabase;

public class SeedDatabaseCommandHandler : ICommandHandler<SeedDatabaseCommand, string>
{
    private readonly IDatabaseSeeder _seeder;
    private readonly ILogger<SeedDatabaseCommandHandler> _logger;

    public SeedDatabaseCommandHandler(IDatabaseSeeder seeder, ILogger<SeedDatabaseCommandHandler> logger)
    {
        _seeder = seeder;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(SeedDatabaseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database seeding process from API with parameters: {@Request}", request);

        try
        {
            await _seeder.SeedDatabaseAsync(
                request.SoLuongNguoiDung,
                request.SoLuongToaNha,
                request.SoLuongTangMoiToa,
                request.SoLuongCanHoMoiTang,
                request.SoLuongPhuongTien,
                request.SoLuongCuTru,
                request.SoLuongChiSoTieuThuMoiCanHo,
                request.SoLuongThePhuongTien,
                request.SoLuongTangHamMoiToa);

            _logger.LogInformation("Database Seeding Completed Successfully.");
            return "Database seeding completed successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding.");
            return new Error("Seeder.Failed", "Failed to seed database. " + ex.Message);
        }
    }
}
