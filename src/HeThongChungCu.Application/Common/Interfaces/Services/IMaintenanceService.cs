namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IMaintenanceService
{
    bool IsMaintenanceActive();
    void SetMaintenanceMode(bool active);
}
