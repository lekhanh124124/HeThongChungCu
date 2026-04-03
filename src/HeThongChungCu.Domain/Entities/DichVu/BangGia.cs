using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class BangGia : AuditableEntity
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

    internal BangGia(
        int dichVuId,
        string tenBangGia,
        DateTime ngayApDung,
        LoaiDinhGia loaiDinhGiaId,
        decimal donGia = 0)
    {
        if (string.IsNullOrWhiteSpace(tenBangGia))
            throw new BusinessException("Tên bảng giá không được để trống.");

        if (loaiDinhGiaId == LoaiDinhGia.LuyTien && donGia != 0)
            throw new BusinessException("Bảng giá lũy tiến không sử dụng đơn giá tổng quát. Đơn giá phải bằng 0.");

        if (loaiDinhGiaId != LoaiDinhGia.LuyTien && donGia < 0)
            throw new BusinessException("Đơn giá không được nhỏ hơn 0.");

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

    public bool IsOverlapping(DateTime requestNgayApDung, DateTime? requestNgayKetThuc)
    {
        if (!IsActive) return false;

        var effectiveEnd = NgayKetThuc ?? DateTime.MaxValue;
        var requestEffectiveEnd = requestNgayKetThuc ?? DateTime.MaxValue;

        return requestNgayApDung <= effectiveEnd && requestEffectiveEnd >= NgayApDung;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;


    public void ClearLuyTien()
    {
        _bangGiaLuyTiens.Clear();
    }

    public void AddLuyTien(decimal tuMuc, decimal? denMuc, decimal donGia)
    {
        if (LoaiDinhGiaId != LoaiDinhGia.LuyTien)
            throw new BusinessException("Bảng giá này không phải loại lũy tiến.");

        var luyTien = new BangGiaLuyTien(Id, tuMuc, denMuc, donGia);

        var previous = _bangGiaLuyTiens.LastOrDefault();
        if (previous == null)
        {
            if (luyTien.TuMuc != 0)
                throw new BusinessException("Bậc đầu tiên phải bắt đầu từ 0.");
        }
        else
        {
            if (luyTien.TuMuc != previous.DenMuc)
                throw new BusinessException("Các bậc thang phải liên tục (không có khoảng trống hoặc chồng lấn).");
        }

        _bangGiaLuyTiens.Add(luyTien);
    }
}
