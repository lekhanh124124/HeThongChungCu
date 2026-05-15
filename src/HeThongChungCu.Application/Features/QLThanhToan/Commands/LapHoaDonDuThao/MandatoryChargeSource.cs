
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.Enums;
using System.Linq;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public class MandatoryChargeSource : IChargeSource
{
    private readonly IBillingDomainService _billingService;

    public MandatoryChargeSource(IBillingDomainService billingService)
    {
        _billingService = billingService;
    }

    public bool AttachCharges(HoaDon hoaDon, Domain.Entities.CanHo canHo, BillingDataBundle data)
    {
        bool hasAdded = false;
        foreach (var svc in data.MandatoryServices)
        {
            var bg = svc.BangGias.FirstOrDefault(b => b.IsActive && b.IsDinhKy);
            if (bg == null) continue;

            // Skip if this is a consumption-based service (Lũy tiến)
            // or if it's already handled by other specific sources
            if (bg.LoaiDinhGiaId == LoaiDinhGia.LuyTien) continue;
            if (data.ConsumptionRecords[canHo.Id].Any(r => r.DichVuId == svc.Id)) continue;
            if (data.Subscriptions[canHo.Id].Any(s => s.DichVuId == svc.Id)) continue;

            if (svc.LoaiDichVuId == LoaiDichVu.ThueNha)
            {
                if (canHo.TinhTrangCanHoId == TrangThaiCanHo.DangChoThue)
                {
                    var relations = data.ResidencyRelations[canHo.Id];
                    _billingService.AttachRentDetail(hoaDon, canHo, relations, bg);
                    hasAdded = true;
                }
            }
            else
            {
                // General operational fees apply to both owners and renters from investor
                if (canHo.TinhTrangCanHoId == TrangThaiCanHo.DaBanGiao || canHo.TinhTrangCanHoId == TrangThaiCanHo.DangChoThue)
                {
                    _billingService.AttachMandatoryFeeDetail(hoaDon, canHo, bg);
                    hasAdded = true;
                }
            }
        }
        return hasAdded;
    }
}
