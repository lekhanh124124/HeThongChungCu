using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public class SubscriptionChargeSource : IChargeSource
{
    private readonly IBillingDomainService _billingService;

    public SubscriptionChargeSource(IBillingDomainService billingService)
    {
        _billingService = billingService;
    }

    public bool AttachCharges(HoaDon hoaDon, Domain.Entities.CanHo canHo, BillingDataBundle data)
    {
        bool hasAdded = false;
        foreach (var sub in data.Subscriptions[canHo.Id])
        {
            if (data.PeriodicServiceDict.TryGetValue(sub.DichVuId, out var svc))
            {
                var bg = svc.BangGias.FirstOrDefault(b => b.IsActive && b.IsDinhKy);
                if (bg != null)
                {
                    _billingService.AttachRecurringDetail(hoaDon, sub, canHo, bg);
                    hasAdded = true;
                }
            }
        }
        return hasAdded;
    }
}
