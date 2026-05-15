using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;

namespace HeThongChungCu.Domain.Entities;

public class GiaoDichThanhToan : AggregateRoot
{
    public int ChiTietHoaDonId { get; private set; }
    public DateTimeOffset NgayGiaoDich { get; private set; }
    public decimal SoTien { get; private set; }
    public PhuongThucThanhToan PhuongThucThanhToanId { get; private set; } = null!;
    public string? MaGiaoDich { get; private set; }
    public string? GhiChu { get; private set; }

    private GiaoDichThanhToan() { } // EF Core

    private GiaoDichThanhToan(
        int chiTietHoaDonId,
        DateTimeOffset ngayGiaoDich,
        decimal soTien,
        PhuongThucThanhToan phuongThucThanhToanId,
        string? maGiaoDich,
        string? ghiChu)
    {
        ChiTietHoaDonId = chiTietHoaDonId;
        NgayGiaoDich = ngayGiaoDich;
        SoTien = soTien;
        PhuongThucThanhToanId = phuongThucThanhToanId;
        MaGiaoDich = maGiaoDich;
        GhiChu = ghiChu;
    }

    public static Result<GiaoDichThanhToan> RecordTransaction(
        int chiTietHoaDonId,
        decimal soTien,
        PhuongThucThanhToan phuongThucThanhToanId,
        string? maGiaoDich = null,
        string? ghiChu = null)
    {
        if (soTien <= 0)
            return Result.Failure<GiaoDichThanhToan>(GiaoDichErrors.InvalidAmount);

        var transaction = new GiaoDichThanhToan(
            chiTietHoaDonId,
            DateTimeOffset.Now,
            soTien,
            phuongThucThanhToanId,
            maGiaoDich,
            ghiChu);

        transaction.AddDomainEvent(new GiaoDichThanhToanRecordedEvent(transaction));

        return Result.Success(transaction);
    }
}
