using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public abstract class BangGia : AuditableEntity
{
    public int DichVuId { get; private set; }
    public string TenBangGia { get; private set; } = string.Empty;
    public ThoiGianHieuLuc ThoiGian { get; private set; } = null!;
    public LoaiDinhGia LoaiDinhGiaId { get; protected set; } = null!;
    public bool IsActive { get; private set; }

    public DichVu DichVu { get; private set; } = null!;

    protected BangGia() { } // EF Core

    protected BangGia(
        int dichVuId,
        string tenBangGia,
        DateTimeOffset ngayApDung,
        LoaiDinhGia loaiDinhGiaId,
        DateTimeOffset? ngayKetThuc = null)
    {
        if (string.IsNullOrWhiteSpace(tenBangGia))
            throw new BusinessException("Tên bảng giá không được để trống.");

        DichVuId = dichVuId;
        TenBangGia = tenBangGia;
        ThoiGian = new ThoiGianHieuLuc(ngayApDung, ngayKetThuc);
        LoaiDinhGiaId = loaiDinhGiaId;
        IsActive = false;
    }

    public bool IsOverlapping(DateTimeOffset requestNgayApDung, DateTimeOffset? requestNgayKetThuc)
    {
        if (!IsActive) return false;

        return ThoiGian.Overlaps(new ThoiGianHieuLuc(requestNgayApDung, requestNgayKetThuc));
    }

    public void Deactivate()
    {
        IsActive = false;
        if (ThoiGian.NgayKetThuc == null)
        {
            ThoiGian = new ThoiGianHieuLuc(ThoiGian.NgayBatDau, DateTimeOffset.Now);
        }
    }

    public void ExpireAt(DateTimeOffset ngayKetThuc)
    {
        ThoiGian = new ThoiGianHieuLuc(ThoiGian.NgayBatDau, ngayKetThuc);
    }

    public void Activate() => IsActive = true;

    /// <summary>
    /// Tính toán số tiền dựa trên ngữ cảnh cung cấp.
    /// </summary>
    /// <param name="context">Ngữ cảnh chứa các thông số đầu vào (số lượng, diện tích, chỉ số...)</param>
    /// <returns>Số tiền tính toán được</returns>
    public abstract decimal CalculateAmount(PricingContext context);
}
