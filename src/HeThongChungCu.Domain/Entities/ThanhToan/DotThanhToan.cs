using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class DotThanhToan : AggregateRoot
{
    public string TenDot { get; private set; } = string.Empty;
    public KyThanhToan KyThanhToan { get; private set; } = null!;
    public TrangThaiDotThanhToan TrangThaiDotThanhToanId { get; private set; } = null!;
    public DateTimeOffset? NgayPhatHanh { get; private set; }
    public string? GhiChu { get; private set; }

    private DotThanhToan() { } // EF Core

    private DotThanhToan(string tenDot, KyThanhToan kyThanhToan, string? ghiChu)
    {
        TenDot = tenDot;
        KyThanhToan = kyThanhToan;
        TrangThaiDotThanhToanId = TrangThaiDotThanhToan.TaoMoi;
        GhiChu = ghiChu;
    }

    public static Result<DotThanhToan> Create(string tenDot, KyThanhToan kyThanhToan, string? ghiChu = null)
    {
        if (string.IsNullOrWhiteSpace(tenDot))
            throw new BusinessException("Tên đợt thanh toán không được để trống.");

        if (kyThanhToan == null)
            throw new BusinessException("Kỳ thanh toán không hợp lệ.");

        return Result.Success(new DotThanhToan(tenDot, kyThanhToan, ghiChu));
    }

    public void Update(string tenDot, KyThanhToan kyThanhToan, string? ghiChu)
    {
        if (TrangThaiDotThanhToanId != TrangThaiDotThanhToan.TaoMoi)
            throw new BusinessException("Chỉ có thể cập nhật đợt thanh toán ở trạng thái Tạo mới.");

        if (string.IsNullOrWhiteSpace(tenDot))
            throw new BusinessException("Tên đợt thanh toán không được để trống.");

        if (kyThanhToan == null)
            throw new BusinessException("Kỳ thanh toán không hợp lệ.");

        TenDot = tenDot;
        KyThanhToan = kyThanhToan;
        GhiChu = ghiChu;
    }

    public void MarkAsDraftGenerated()
    {
        if (TrangThaiDotThanhToanId != TrangThaiDotThanhToan.DaDuyet)
            throw new BusinessException("Chỉ có thể lập hóa đơn dự thảo cho đợt thanh toán ở trạng thái Đã duyệt.");

        TrangThaiDotThanhToanId = TrangThaiDotThanhToan.DaLapDuThao;
    }

    public void MarkAsIssued()
    {
        if (TrangThaiDotThanhToanId != TrangThaiDotThanhToan.DaLapDuThao)
            throw new BusinessException("Chỉ có thể phát hành đợt thanh toán ở trạng thái Đã lập dự thảo.");

        TrangThaiDotThanhToanId = TrangThaiDotThanhToan.DaPhatHanh;
        NgayPhatHanh = DateTimeOffset.Now;
    }

    public void MarkAsApproved()
    {
        if (TrangThaiDotThanhToanId != TrangThaiDotThanhToan.TaoMoi)
            throw new BusinessException("Chỉ có thể duyệt đợt thanh toán ở trạng thái Tạo mới.");

        TrangThaiDotThanhToanId = TrangThaiDotThanhToan.DaDuyet;
    }

    public Result MarkAsClosed()
    {
        if (TrangThaiDotThanhToanId != TrangThaiDotThanhToan.DaPhatHanh)
            return Result.Failure(DotThanhToanErrors.CannotClose);

        TrangThaiDotThanhToanId = TrangThaiDotThanhToan.DaDong;

        return Result.Success();
    }
}

