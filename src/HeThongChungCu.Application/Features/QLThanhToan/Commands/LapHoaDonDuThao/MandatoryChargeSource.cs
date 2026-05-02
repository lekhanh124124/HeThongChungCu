using HeThongChungCu.Domain.Constants;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Interfaces;

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

            if (svc.MaDichVu == ServiceCodeConstants.TIEN_THUE_NHA)
            {
                var relations = data.ResidencyRelations[canHo.Id];
                _billingService.AttachRentDetail(hoaDon, canHo, relations, bg);
            }
            else
            {
                _billingService.AttachMandatoryFeeDetail(hoaDon, canHo, bg);
            }
            hasAdded = true;
        }
        return hasAdded;
    }
}
