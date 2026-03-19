using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class BangGia : AggregateRoot
{
    public int DichVuId { get; private set; }

    public string TenBangGia { get; private set; } = string.Empty;
    public DateTime NgayApDung { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public decimal DonGia { get; private set; }
    public LoaiDinhGia LoaiDinhGiaId { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private readonly List<BangGiaLuyTien> _bangGiaLuyTiens = [];
    public IReadOnlyCollection<BangGiaLuyTien> BangGiaLuyTiens => _bangGiaLuyTiens.AsReadOnly();

    private BangGia() { } // EF Core

    public BangGia(
        int dichVuId,
        string tenBangGia,
        DateTime ngayApDung,
        LoaiDinhGia loaiDinhGiaId,
        decimal donGia = 0)
    {
        if (string.IsNullOrWhiteSpace(tenBangGia))
            throw new BusinessException("Tên bảng giá không được để trống.");

        DichVuId = dichVuId;
        TenBangGia = tenBangGia;
        NgayApDung = ngayApDung;
        LoaiDinhGiaId = loaiDinhGiaId;
        DonGia = donGia;
        IsActive = true;
    }

    public void UpdateInfo(string tenBangGia, DateTime ngayApDung, DateTime? ngayKetThuc, decimal donGia, LoaiDinhGia loaiDinhGiaId)
    {
        TenBangGia = tenBangGia;
        NgayApDung = ngayApDung;
        NgayKetThuc = ngayKetThuc;
        DonGia = donGia;
        LoaiDinhGiaId = loaiDinhGiaId;
    }

    // public bool IsOverlapping(DateTime requestNgayApDung, DateTime? requestNgayKetThuc)
    // {
    //     if (!IsActive) return false;

    //     // Existing: [NgayApDung, NgayKetThuc]
    //     // Request: [requestNgayApDung, requestNgayKetThuc]

    //     var effectiveEnd = NgayKetThuc ?? DateTime.MaxValue;
    //     var requestEffectiveEnd = requestNgayKetThuc ?? DateTime.MaxValue;

    //     return requestNgayApDung <= effectiveEnd && requestEffectiveEnd >= NgayApDung;
    // }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void AddLuyTien(double tuMuc, double? denMuc, decimal donGia)
    {
        if (LoaiDinhGiaId != LoaiDinhGia.LuyTien)
            throw new BusinessException("Bảng giá này không phải loại lũy tiến.");

        var luyTien = new BangGiaLuyTien(Id, tuMuc, denMuc, donGia);

        // Validate tiers
        if (_bangGiaLuyTiens.Count != 0)
        {
            var lastTier = _bangGiaLuyTiens.Last();
            if (tuMuc < (lastTier.DenMuc ?? double.MaxValue))
                throw new BusinessException("Bậc thang mới phải bắt đầu sau bậc thang trước đó.");
        }

        _bangGiaLuyTiens.Add(luyTien);
    }
}