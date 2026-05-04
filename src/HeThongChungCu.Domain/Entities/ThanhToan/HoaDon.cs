using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.ValueObjects;
using HeThongChungCu.Domain.Events;

namespace HeThongChungCu.Domain.Entities;

public class HoaDon : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int? DotThanhToanId { get; private set; }
    public string MaHoaDon { get; private set; } = null!;
    public KyThanhToan KyThanhToan { get; private set; } = null!;
    public DateTimeOffset NgayLap { get; private set; }
    public DateTimeOffset NgayHanThanhToan { get; private set; }
    public decimal TongTien { get; private set; }
    public TrangThaiHoaDon TrangThaiHoaDonId { get; private set; } = null!;
    public string? GhiChu { get; private set; }

    /// <summary>
    /// Ngày lần cuối tính lãi trễ hạn cho hóa đơn này.
    /// Null = chưa bao giờ bị tính lãi. Dùng để tránh tính lãi trùng qua nhiều kỳ thanh toán.
    /// </summary>
    public DateTimeOffset? NgayTinhLaiCuoi { get; private set; }

    /// <summary>
    /// Ghi nhận thời điểm đã tính lãi trễ hạn. Được gọi sau khi LapHoaDonDuThao gắn lãi vào hóa đơn mới.
    /// </summary>
    public void SetNgayTinhLai(DateTimeOffset ngayTinh) => NgayTinhLaiCuoi = ngayTinh;

    private readonly List<ChiTietHoaDon> _chiTietHoaDons = [];
    public IReadOnlyCollection<ChiTietHoaDon> ChiTietHoaDons => _chiTietHoaDons.AsReadOnly();

    private HoaDon() { } // EF Core

    private HoaDon(
        int canHoId,
        int? dotThanhToanId,
        string maHoaDon,
        KyThanhToan kyThanhToan,
        DateTimeOffset ngayLap,
        DateTimeOffset ngayHanThanhToan,
        string? ghiChu)
    {
        CanHoId = canHoId;
        DotThanhToanId = dotThanhToanId;
        MaHoaDon = maHoaDon;
        KyThanhToan = kyThanhToan;
        NgayLap = ngayLap;
        NgayHanThanhToan = ngayHanThanhToan;
        TrangThaiHoaDonId = TrangThaiHoaDon.ChoDuyet;
        GhiChu = ghiChu;
        TongTien = 0;
    }

    public static Result<HoaDon> CreateHoaDon(
        int canHoId,
        int? dotThanhToanId,
        string maHoaDon,
        KyThanhToan kyThanhToan,
        DateTimeOffset ngayLap,
        DateTimeOffset ngayHanThanhToan,
        string? ghiChu = null)
    {
        if (string.IsNullOrWhiteSpace(maHoaDon))
            return Result.Failure<HoaDon>(HoaDonErrors.MaHoaDonRequired);

        if (kyThanhToan == null)
            return Result.Failure<HoaDon>(HoaDonErrors.InvalidBillingPeriod);

        if (ngayHanThanhToan < ngayLap)
            return Result.Failure<HoaDon>(HoaDonErrors.InvalidDueDate);

        var hoaDon = new HoaDon(canHoId, dotThanhToanId, maHoaDon, kyThanhToan, ngayLap, ngayHanThanhToan, ghiChu);

        hoaDon.AddDomainEvent(new HoaDonCreatedEvent(hoaDon));

        return Result.Success(hoaDon);
    }

    public Result AddChiTiet(ChiTietHoaDon chiTiet)
    {
        if (TrangThaiHoaDonId != TrangThaiHoaDon.ChoDuyet)
            return Result.Failure(HoaDonErrors.CannotModifyIssuedInvoice);

        _chiTietHoaDons.Add(chiTiet);
        RecalculateTotal();

        return Result.Success();
    }

    public Result AddDichVuDetail(string tenMucPhi, decimal soLuong, decimal donGia, int dichVuId, string? ghiChu = null)
    {
        var detail = new ChiTietHoaDonDichVu(Id, tenMucPhi, soLuong, donGia, dichVuId, ghiChu);
        return AddChiTiet(detail);
    }

    public Result AddTieuThuDetail(string tenMucPhi, decimal chiSoCu, decimal chiSoMoi, decimal donGia, int dichVuId, string? ghiChu = null)
    {
        var detail = new ChiTietHoaDonTieuThu(Id, tenMucPhi, chiSoCu, chiSoMoi, donGia, dichVuId, ghiChu);
        return AddChiTiet(detail);
    }

    public Result AddSuaChuaDetail(int yeuCauSuaChuaId, string tenMucPhi, decimal soTien, string? ghiChu = null)
    {
        var detail = new ChiTietHoaDonSuaChua(Id, yeuCauSuaChuaId, tenMucPhi, soTien, ghiChu);
        return AddChiTiet(detail);
    }

    public Result AddThiCongDetail(int yeuCauThiCongId, LoaiChiPhiThiCong loai, string tenMucPhi, decimal soTien, string? ghiChu = null)
    {
        var detail = new ChiTietHoaDonThiCong(Id, yeuCauThiCongId, loai, tenMucPhi, soTien, ghiChu);
        return AddChiTiet(detail);
    }

    public void UpdateStatus(TrangThaiHoaDon status)
    {
        TrangThaiHoaDonId = status;
    }

    public Result PhatHanh()
    {
        if (TrangThaiHoaDonId != TrangThaiHoaDon.ChoDuyet)
            return Result.Failure(HoaDonErrors.InvalidStatusTransition);

        if (_chiTietHoaDons.Count == 0)
            return Result.Failure(HoaDonErrors.InvoiceHasNoDetails);

        TrangThaiHoaDonId = TrangThaiHoaDon.ChuaThanhToan;

        return Result.Success();
    }

    private void RecalculateTotal()
    {
        TongTien = _chiTietHoaDons.Sum(x => x.ThanhTien);
    }
}
