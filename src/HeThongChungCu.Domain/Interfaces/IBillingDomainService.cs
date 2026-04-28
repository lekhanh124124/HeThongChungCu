using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Interfaces;

public interface IBillingDomainService
{
    Result PhatHanhBatch(DotThanhToan dotThanhToan, IEnumerable<HoaDon> hoaDons);
    
    Result<HoaDon> CreateInvoiceHeader(CanHo canHo, DotThanhToan dot, KyThanhToan ky, string maHoaDon, DateTimeOffset ngayHanThanhToan);

    // Các hàm Smart Attach: Tự tính tiền và gắn vào hóa đơn
    void AttachConsumptionDetail(HoaDon hoaDon, ChiSoTieuThu chiSo, BangGia bangGia);
    
    void AttachRecurringDetail(HoaDon hoaDon, DangKyDichVu dangKy, CanHo canHo, BangGia bangGia);
    
    void AttachRentDetail(HoaDon hoaDon, CanHo canHo, IEnumerable<QuanHeCuTru> residencyRelations, BangGia bangGia);

    void AttachMandatoryFeeDetail(HoaDon hoaDon, CanHo canHo, BangGia bangGia);

    void AttachRepairDetail(HoaDon hoaDon, YeuCauSuaChua suaChua, BangGia? bangGia = null);
    
    void AttachConstructionDetail(HoaDon hoaDon, YeuCauThiCong thiCong, LoaiChiPhiThiCong loai, BangGia? bangGia = null);

    void AttachLateInterestDetail(HoaDon hoaDon, HoaDon overdueInvoice, BangGia interestBangGia, DateTimeOffset calculationDate);
}
