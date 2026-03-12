using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.Seeder.Commands.SeedDatabase;

public record SeedDatabaseCommand(
    int NumberOfUsers = 10,
    int NumberOfBuildings = 3,
    int NumberOfFloorsPerBuilding = 10,
    int NumberOfApartmentsPerFloor = 5,
    int NumberOfVehicles = 30) : ICommand<string>;
