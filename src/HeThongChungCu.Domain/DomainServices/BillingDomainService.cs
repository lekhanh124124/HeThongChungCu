using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.DomainServices;

public class BillingDomainService : IBillingDomainService
{
    public Result PhatHanhBatch(DotThanhToan dotThanhToan, IEnumerable<HoaDon> hoaDons)
    {
        if (dotThanhToan.TrangThaiDotThanhToanId != TrangThaiDotThanhToan.Nhap)
        {
            return Result.Failure(HoaDonErrors.InvalidBatchStatus);
        }

        foreach (var hoaDon in hoaDons)
        {
            var result = hoaDon.PhatHanh();
            if (result.IsFailure) return result;
        }

        dotThanhToan.MarkAsIssued();
        dotThanhToan.AddDomainEvent(new DotThanhToanPhatHanhEvent(dotThanhToan, hoaDons));

        return Result.Success();
    }

    public Result<HoaDon> CreateInvoiceHeader(CanHo canHo, DotThanhToan dot, KyThanhToan ky, string maHoaDon, DateTimeOffset ngayHanThanhToan)
    {
        return HoaDon.CreateHoaDon(
            canHo.Id,
            dot.Id,
            maHoaDon,
            ky,
            DateTimeOffset.Now,
            ngayHanThanhToan
        );
    }

    public void AttachConsumptionDetail(HoaDon hoaDon, ChiSoTieuThu chiSo, BangGia bangGia)
    {
        var amount = CalculateConsumptionFee(chiSo, bangGia);
        var tenDichVu = bangGia.DichVu?.TenDichVu ?? "Điện/Nước";

        hoaDon.AddTieuThuDetail(
            $"Tiền {tenDichVu} tháng {chiSo.Thang}/{chiSo.Nam}",
            chiSo.ChiSoCu,
            chiSo.ChiSoMoi,
            amount / (chiSo.SoLuong == 0 ? 1 : chiSo.SoLuong),
            bangGia.DichVuId
        );
    }

    public void AttachRecurringDetail(HoaDon hoaDon, DangKyDichVu dangKy, CanHo canHo, BangGia bangGia)
    {
        var amount = CalculateRecurringFee(dangKy, canHo, bangGia);
        var tenDichVu = bangGia.DichVu?.TenDichVu ?? "Dịch vụ";

        hoaDon.AddDichVuDetail(
            $"Phí {tenDichVu} tháng {dangKy.ThoiGian.NgayBatDau.Month}/{dangKy.ThoiGian.NgayBatDau.Year}",
            1,
            amount,
            bangGia.DichVuId
        );
    }

    public void AttachRentDetail(HoaDon hoaDon, CanHo canHo, IEnumerable<QuanHeCuTru> residencyRelations, BangGia bangGia)
    {
        var amount = CalculateRent(canHo, residencyRelations, bangGia);
        if (amount <= 0) return;

        hoaDon.AddDichVuDetail(
            $"Tiền thuê nhà {canHo.MaCanHo} - {bangGia.TenBangGia}",
            1,
            amount,
            bangGia.DichVuId
        );
    }

    public void AttachMandatoryFeeDetail(HoaDon hoaDon, CanHo canHo, BangGia bangGia)
    {
        var amount = CalculateMandatoryFee(canHo, bangGia);
        if (amount <= 0) return;

        hoaDon.AddDichVuDetail(
            $"Phí {bangGia.DichVu?.TenDichVu ?? "Bắt buộc"} - {canHo.MaCanHo}",
            canHo.ThongSo.DienTich,
            amount / (canHo.ThongSo.DienTich == 0 ? 1 : canHo.ThongSo.DienTich),
            bangGia.DichVuId
        );
    }

    public void AttachRepairDetail(HoaDon hoaDon, YeuCauSuaChua suaChua, BangGia? bangGia = null)
    {
        if (suaChua.IsMienPhi == true) return;

        decimal amount = 0;
        if (suaChua.ChiPhiThucTe.HasValue)
        {
            amount = suaChua.ChiPhiThucTe.Value;
        }
        else if (bangGia != null)
        {
            amount = bangGia.CalculateAmount(new PricingContext(SoLuong: 1));
        }

        if (amount <= 0) return;

        hoaDon.AddSuaChuaDetail(
            suaChua.Id,
            $"Phí sửa chữa - {suaChua.NoiDung}",
            amount
        );
    }

    public void AttachConstructionDetail(HoaDon hoaDon, YeuCauThiCong thiCong, LoaiChiPhiThiCong loai, BangGia? bangGia = null)
    {
        decimal amount = 0;
        if (loai == LoaiChiPhiThiCong.DatCoc)
            amount = thiCong.TienDatCoc ?? 0;
        else if (loai == LoaiChiPhiThiCong.PhatViPham)
            amount = thiCong.TienKhauTru ?? 0;

        if (amount <= 0 && bangGia != null)
        {
            amount = bangGia.CalculateAmount(new PricingContext(SoLuong: 1));
        }

        if (amount <= 0) return;

        hoaDon.AddThiCongDetail(
            thiCong.Id,
            loai,
            $"{loai.Name} - {thiCong.HangMucThiCong}",
            amount
        );
    }

    public void AttachLateInterestDetail(HoaDon hoaDon, HoaDon overdueInvoice, BangGia interestBangGia, DateTimeOffset calculationDate)
    {
        var amount = CalculateLateInterest(overdueInvoice, interestBangGia, calculationDate);
        if (amount <= 0) return;

        hoaDon.AddDichVuDetail(
            $"Lãi chậm nộp cho hóa đơn {overdueInvoice.MaHoaDon}",
            1,
            amount,
            interestBangGia.DichVuId,
            $"Quá hạn từ {overdueInvoice.NgayHanThanhToan:dd/MM/yyyy}"
        );
    }

    // --- Private Calculation Methods ---

    private decimal CalculateConsumptionFee(ChiSoTieuThu chiSo, BangGia bangGia)
    {
        var context = new PricingContext(
            SoLuong: chiSo.SoLuong,
            ChiSoDau: chiSo.ChiSoCu,
            ChiSoCuoi: chiSo.ChiSoMoi
        );

        return bangGia.CalculateAmount(context);
    }

    private decimal CalculateRecurringFee(DangKyDichVu dangKy, CanHo canHo, BangGia bangGia)
    {
        var context = new PricingContext(
            SoLuong: dangKy.SoLuong,
            DienTich: canHo.ThongSo.DienTich,
            LoaiCanHoId: canHo.LoaiCanHoId,
            KhungGioId: dangKy.KhungGioId
        );

        return bangGia.CalculateAmount(context);
    }

    private decimal CalculateRent(CanHo canHo, IEnumerable<QuanHeCuTru> residencyRelations, BangGia bangGia)
    {
        bool hasActiveTenant = residencyRelations.Any(r =>
            r.CanHoId == canHo.Id &&
            r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue &&
            r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);

        if (!hasActiveTenant) return 0;

        var context = new PricingContext(
            SoLuong: 1,
            LoaiCanHoId: canHo.LoaiCanHoId
        );

        return bangGia.CalculateAmount(context);
    }

    private decimal CalculateMandatoryFee(CanHo canHo, BangGia bangGia)
    {
        var area = canHo.ThongSo.DienTich;
        var context = new PricingContext(
            SoLuong: area,
            DienTich: area,
            LoaiCanHoId: canHo.LoaiCanHoId
        );

        return bangGia.CalculateAmount(context);
    }

    private decimal CalculateLateInterest(HoaDon overdueInvoice, BangGia interestBangGia, DateTimeOffset calculationDate)
    {
        if (overdueInvoice.TrangThaiHoaDonId == TrangThaiHoaDon.DaThanhToan) return 0;
        if (calculationDate <= overdueInvoice.NgayHanThanhToan) return 0;

        var overdueDays = (calculationDate - overdueInvoice.NgayHanThanhToan).Days;
        if (overdueDays <= 0) return 0;

        var interestBase = overdueInvoice.TongTien * overdueDays;
        var context = new PricingContext(
            SoLuong: interestBase,
            SoTienGoc: overdueInvoice.TongTien,
            SoNgayQuaHan: overdueDays
        );

        return interestBangGia.CalculateAmount(context);
    }
}
