using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public class VehicleChargeSource : IChargeSource
{
    private readonly IBillingDomainService _billingService;

    public VehicleChargeSource(IBillingDomainService billingService)
    {
        _billingService = billingService;
    }

    public bool AttachCharges(HoaDon hoaDon, HeThongChungCu.Domain.Entities.CanHo canHo, BillingDataBundle data)
    {
        bool hasAdded = false;
        var vehicles = data.ActiveVehicles[canHo.Id];

        foreach (var vehicle in vehicles)
        {
            var serviceCode = vehicle.LoaiPhuongTienId.DefaultServiceCode;
            
            // Tìm dịch vụ tương ứng trong bundle
            var service = data.PeriodicServiceDict.Values
                .FirstOrDefault(s => s.MaDichVu == serviceCode);

            if (service != null)
            {
                var bg = service.BangGias.FirstOrDefault(b => b.IsActive && b.IsDinhKy);
                if (bg != null)
                {
                    _billingService.AttachVehicleDetail(hoaDon, vehicle, bg);
                    hasAdded = true;
                }
            }
        }

        return hasAdded;
    }
}
