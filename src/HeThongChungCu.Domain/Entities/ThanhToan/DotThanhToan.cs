using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class DotThanhToan : AggregateRoot
{
    public string TenDot { get; private set; } = null!;
    public KyThanhToan KyThanhToan { get; private set; } = null!;
    public TrangThaiDotThanhToan TrangThaiDotThanhToanId { get; private set; } = null!;
    public DateTimeOffset? NgayPhatHanh { get; private set; }
    public string? GhiChu { get; private set; }

    private DotThanhToan() { } // EF Core

    private DotThanhToan(string tenDot, KyThanhToan kyThanhToan, string? ghiChu)
    {
        TenDot = tenDot;
        KyThanhToan = kyThanhToan;
        TrangThaiDotThanhToanId = TrangThaiDotThanhToan.Nhap;
        GhiChu = ghiChu;
    }

    public static Result<DotThanhToan> Create(string tenDot, KyThanhToan kyThanhToan, string? ghiChu = null)
    {
        if (string.IsNullOrWhiteSpace(tenDot))
            return Result.Failure<DotThanhToan>(new Error("DotThanhToan.TenDotRequired", "Tên đợt thanh toán không được để trống."));

        if (kyThanhToan == null)
            return Result.Failure<DotThanhToan>(new Error("DotThanhToan.KyThanhToanRequired", "Kỳ thanh toán không hợp lệ."));

        return Result.Success(new DotThanhToan(tenDot, kyThanhToan, ghiChu));
    }

    public void MarkAsIssued()
    {
        TrangThaiDotThanhToanId = TrangThaiDotThanhToan.DaPhatHanh;
        NgayPhatHanh = DateTimeOffset.Now;
    }

    public void MarkAsClosed()
    {
        TrangThaiDotThanhToanId = TrangThaiDotThanhToan.DaDong;
    }
}
