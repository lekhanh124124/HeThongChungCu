namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IDatabaseSeeder
{
    Task SeedDatabaseAsync(
        int numberOfUsers, 
        int numberOfBuildings, 
        int numberOfFloorsPerBuilding, 
        int numberOfApartmentsPerFloor, 
        int numberOfVehicles);
}
