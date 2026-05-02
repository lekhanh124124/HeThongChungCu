using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public interface IChargeSource
{
    bool AttachCharges(HoaDon hoaDon, Domain.Entities.CanHo canHo, BillingDataBundle data);
}
