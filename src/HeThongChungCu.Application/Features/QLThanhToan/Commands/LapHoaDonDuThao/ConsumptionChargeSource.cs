using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public class ConsumptionChargeSource : IChargeSource
{
    private readonly IBillingDomainService _billingService;
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;

    public ConsumptionChargeSource(IBillingDomainService billingService, IChiSoTieuThuCommandRepository chiSoRepository)
    {
        _billingService = billingService;
        _chiSoRepository = chiSoRepository;
    }

    public List<(HoaDon HoaDon, int ChiSoId)> ConsumptionsToLink { get; } = [];

    public bool AttachCharges(HoaDon hoaDon, Domain.Entities.CanHo canHo, BillingDataBundle data)
    {
        bool hasAdded = false;
        foreach (var record in data.ConsumptionRecords[canHo.Id])
        {
            if (data.PeriodicServiceDict.TryGetValue(record.DichVuId, out var svc))
            {
                var bg = svc.BangGias.FirstOrDefault(b => b.IsActive && b.IsDinhKy);
                if (bg != null)
                {
                    _billingService.AttachConsumptionDetail(hoaDon, record, bg);
                    ConsumptionsToLink.Add((hoaDon, record.Id));
                    hasAdded = true;
                }
            }
        }
        return hasAdded;
    }
}
