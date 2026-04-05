using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class BangGia : AuditableEntity
{
    public int DichVuId { get; private set; }
    public string TenBangGia { get; private set; } = string.Empty;
    public ThoiGianHieuLuc ThoiGian { get; private set; } = null!;
    public GiaTien DonGia { get; private set; } = null!;
    public LoaiDinhGia LoaiDinhGiaId { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private readonly List<BangGiaLuyTien> _bangGiaLuyTiens = [];
    public IReadOnlyCollection<BangGiaLuyTien> BangGiaLuyTiens => _bangGiaLuyTiens.AsReadOnly();

    private BangGia() { } // EF Core

    internal BangGia(
        int dichVuId,
        string tenBangGia,
        DateTimeOffset ngayApDung,
        LoaiDinhGia loaiDinhGiaId,
        decimal donGia = 0)
    {
        if (string.IsNullOrWhiteSpace(tenBangGia))
            throw new BusinessException("Tên bảng giá không được để trống.");

        if (loaiDinhGiaId == LoaiDinhGia.LuyTien && donGia != 0)
            throw new BusinessException("Bảng giá lũy tiến không sử dụng đơn giá tổng quát. Đơn giá phải bằng 0.");

        DichVuId = dichVuId;
        TenBangGia = tenBangGia;
        ThoiGian = new ThoiGianHieuLuc(ngayApDung);
        LoaiDinhGiaId = loaiDinhGiaId;
        DonGia = new GiaTien(donGia);
        IsActive = true;
    }

    public void UpdateInfo(string tenBangGia, DateTimeOffset ngayApDung, DateTimeOffset? ngayKetThuc, decimal donGia, LoaiDinhGia loaiDinhGiaId)
    {
        TenBangGia = tenBangGia;
        ThoiGian = new ThoiGianHieuLuc(ngayApDung, ngayKetThuc);
        DonGia = new GiaTien(donGia);
        LoaiDinhGiaId = loaiDinhGiaId;
    }

    public bool IsOverlapping(DateTimeOffset requestNgayApDung, DateTimeOffset? requestNgayKetThuc)
    {
        if (!IsActive) return false;

        return ThoiGian.Overlaps(new ThoiGianHieuLuc(requestNgayApDung, requestNgayKetThuc));
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
