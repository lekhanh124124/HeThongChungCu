using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public record BillingDataBundle(
    Dictionary<int, DichVu> PeriodicServiceDict,
    List<DichVu> MandatoryServices,
    ILookup<int, QuanHeCuTru> ResidencyRelations,
    ILookup<int, ChiSoTieuThu> ConsumptionRecords,
    ILookup<int, DangKyDichVu> Subscriptions,
    ILookup<int, PhuongTien> ActiveVehicles,
    HashSet<int> ExistingInvoiceCanHoIds,
    ILookup<int, HoaDon> OverdueInvoices,
    BangGia? LateInterestBangGia
);
