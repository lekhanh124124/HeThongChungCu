using HeThongChungCu.Application.Features.Seeder.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDatabaseSeeder
{
    Task SeedDatabaseAsync();
}
