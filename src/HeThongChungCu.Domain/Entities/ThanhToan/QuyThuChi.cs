using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Domain.Entities;

public class QuyThuChi : AggregateRoot
{
    public string MaGiaoDich { get; private set; } = null!;
    public decimal TongSoTien { get; private set; }
    public DateTimeOffset NgayGiaoDich { get; private set; }
    public PhuongThucThanhToan PhuongThucThanhToanId { get; private set; } = null!;
    public string NguoiGiaoDich { get; private set; } = null!;
    public string? ChungTuGoc { get; private set; }

    public LoaiThuChi LoaiGiaoDichId { get; private set; } = null!;

    private readonly List<ChiTietQuyThuChi> _chiTiets = new();
    public IReadOnlyCollection<ChiTietQuyThuChi> ChiTiets => _chiTiets.AsReadOnly();

    private QuyThuChi() { } // For EF Core

    private QuyThuChi(
        string maGiaoDich,
        DateTimeOffset ngayGiaoDich,
        PhuongThucThanhToan phuongThucThanhToanId,
        string nguoiGiaoDich,
        LoaiThuChi loaiGiaoDich,
        string? chungTuGoc)
    {
        MaGiaoDich = maGiaoDich;
        NgayGiaoDich = ngayGiaoDich;
        PhuongThucThanhToanId = phuongThucThanhToanId;
        NguoiGiaoDich = nguoiGiaoDich;
        LoaiGiaoDichId = loaiGiaoDich;
        ChungTuGoc = chungTuGoc;
        TongSoTien = 0;
    }

    public static Result<QuyThuChi> CreateThu(
        string maGiaoDich,
        DateTimeOffset ngayGiaoDich,
        PhuongThucThanhToan phuongThucThanhToan,
        string nguoiGiaoDich,
        string? chungTuGoc = null)
    {
        return Create(maGiaoDich, ngayGiaoDich, phuongThucThanhToan, nguoiGiaoDich, LoaiThuChi.Thu, chungTuGoc);
    }

    public static Result<QuyThuChi> CreateChi(
        string maGiaoDich,
        DateTimeOffset ngayGiaoDich,
        PhuongThucThanhToan phuongThucThanhToan,
        string nguoiGiaoDich,
        string? chungTuGoc = null)
    {
        return Create(maGiaoDich, ngayGiaoDich, phuongThucThanhToan, nguoiGiaoDich, LoaiThuChi.Chi, chungTuGoc);
    }

    private static Result<QuyThuChi> Create(
        string maGiaoDich,
        DateTimeOffset ngayGiaoDich,
        PhuongThucThanhToan phuongThucThanhToan,
        string nguoiGiaoDich,
        LoaiThuChi loaiGiaoDich,
        string? chungTuGoc)
    {
        if (string.IsNullOrWhiteSpace(maGiaoDich))
            return Result.Failure<QuyThuChi>(new Error("QuyThuChi.MaGiaoDichRequired", "Mã giao dịch không được để trống."));

        if (ngayGiaoDich > DateTimeOffset.UtcNow.AddMinutes(5)) // Small allowance for clock drift
            return Result.Failure<QuyThuChi>(ThuChiQuyErrors.DateInFuture);

        if (phuongThucThanhToan == null)
            return Result.Failure<QuyThuChi>(ThuChiQuyErrors.PaymentMethodInvalid);

        if (string.IsNullOrWhiteSpace(nguoiGiaoDich))
            return Result.Failure<QuyThuChi>(new Error("QuyThuChi.NguoiGiaoDichRequired", "Người giao dịch không được để trống."));

        return Result.Success(new QuyThuChi(maGiaoDich, ngayGiaoDich, phuongThucThanhToan, nguoiGiaoDich, loaiGiaoDich, chungTuGoc));
    }

    public void AddChiTiet(decimal soTien, string nhomThongKe, string? ghiChu, int? dichVuId = null)
    {
        var chiTiet = new ChiTietQuyThuChi(soTien, nhomThongKe, ghiChu, dichVuId);
        _chiTiets.Add(chiTiet);
        CalculateTongTien();
    }

    public void CalculateTongTien()
    {
        TongSoTien = _chiTiets.Sum(x => x.SoTien);
    }
}
