using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class DichVu : AggregateRoot
{
    public string MaDichVu { get; private set; } = string.Empty;
    public string TenDichVu { get; private set; } = string.Empty;
    public LoaiDichVu LoaiDichVuId { get; private set; } = null!;
    public string DonViTinh { get; private set; } = string.Empty;
    public string? MoTa { get; private set; }
    public bool IsBatBuoc { get; private set; }
    public int? SoLuongToiDa { get; private set; }
    public TrangThaiDichVu TrangThaiId { get; private set; } = null!;
    public int? IconId { get; private set; }
    public TepTaiLieu? Icon { get; private set; }

    private readonly List<BangGia> _bangGias = [];
    public IReadOnlyCollection<BangGia> BangGias => _bangGias.AsReadOnly();

    private readonly List<KhungGioDichVu> _khungGios = [];
    public IReadOnlyCollection<KhungGioDichVu> KhungGios => _khungGios.AsReadOnly();

    private DichVu() { } // EF Core

    public DichVu(
        string maDichVu,
        string tenDichVu,
        LoaiDichVu loaiDichVuId,
        string donViTinh,
        string? moTa = null,
        int? iconId = null,
        bool isBatBuoc = false,
        int? soLuongToiDa = null)
    {
        if (string.IsNullOrWhiteSpace(maDichVu))
            throw new BusinessException("Mã dịch vụ không được để trống.");
        if (string.IsNullOrWhiteSpace(tenDichVu))
            throw new BusinessException("Tên dịch vụ không được để trống.");

        MaDichVu = maDichVu;
        TenDichVu = tenDichVu;
        LoaiDichVuId = loaiDichVuId;
        DonViTinh = donViTinh;
        MoTa = moTa;
        IconId = iconId;
        IsBatBuoc = isBatBuoc;
        SoLuongToiDa = soLuongToiDa;
        TrangThaiId = TrangThaiDichVu.TaoMoi;
    }

    public void Update(
        string tenDichVu,
        LoaiDichVu loaiDichVuId,
        string donViTinh,
        string? moTa,
        int? iconId,
        bool isBatBuoc,
        int? soLuongToiDa)
    {
        TenDichVu = tenDichVu;
        LoaiDichVuId = loaiDichVuId;
        DonViTinh = donViTinh;
        MoTa = moTa;
        IconId = iconId;
        IsBatBuoc = isBatBuoc;
        SoLuongToiDa = soLuongToiDa;
    }

    public Result<KhungGioDichVu> AddKhungGio(TimeSpan gioBatDau, TimeSpan gioKetThuc, string tenKhungGio, NgayTrongTuan? ngayTrongTuan = null)
    {
        if (_khungGios.Any(x => x.OverlapsWith(gioBatDau, gioKetThuc, ngayTrongTuan)))
        {
            return Result.Failure<KhungGioDichVu>(DichVuErrors.KhungGioOverlap);
        }

        var khungGio = new KhungGioDichVu(Id, gioBatDau, gioKetThuc, tenKhungGio, ngayTrongTuan);
        _khungGios.Add(khungGio);

        return Result.Success(khungGio);
    }

    public void Revoke() => TrangThaiId = TrangThaiDichVu.NgungCungCap;
    public void Activate() => TrangThaiId = TrangThaiDichVu.HoatDong;
    public void SetCanhBao() => TrangThaiId = TrangThaiDichVu.CanhBao;

    // --- Price Management ---
    public void AddBangGiaCoDinh(
        string tenBangGia,
        DateTimeOffset ngayApDung,
        decimal donGia,
        LoaiDinhGia? loaiDinhGia = null,
        DateTimeOffset? ngayKetThuc = null)
    {
        EnsureNoOverlap(ngayApDung, ngayKetThuc);
        var newBangGia = new BangGiaCoDinh(Id, tenBangGia, ngayApDung, donGia, loaiDinhGia, ngayKetThuc);
        _bangGias.Add(newBangGia);
    }

    public void AddBangGiaLuyTien(
        string tenBangGia,
        DateTimeOffset ngayApDung,
        DateTimeOffset? ngayKetThuc = null)
    {
        EnsureNoOverlap(ngayApDung, ngayKetThuc);
        var newBangGia = new BangGiaLuyTien(Id, tenBangGia, ngayApDung, ngayKetThuc);
        _bangGias.Add(newBangGia);
    }

    public void AddBangGiaKhungGio(
        string tenBangGia,
        DateTimeOffset ngayApDung,
        DateTimeOffset? ngayKetThuc = null)
    {
        EnsureNoOverlap(ngayApDung, ngayKetThuc);
        var newBangGia = new BangGiaKhungGio(Id, tenBangGia, ngayApDung, ngayKetThuc);
        _bangGias.Add(newBangGia);
    }

    public void AddBangGiaLoaiCanHo(
        string tenBangGia,
        DateTimeOffset ngayApDung,
        DateTimeOffset? ngayKetThuc = null)
    {
        EnsureNoOverlap(ngayApDung, ngayKetThuc);
        var newBangGia = new BangGiaLoaiCanHo(Id, tenBangGia, ngayApDung, ngayKetThuc);
        _bangGias.Add(newBangGia);
    }

    private void EnsureNoOverlap(DateTimeOffset ngayApDung, DateTimeOffset? ngayKetThuc)
    {
        var newPeriod = new ThoiGianHieuLuc(ngayApDung, ngayKetThuc);

        foreach (var bg in _bangGias.Where(bg => bg.IsActive))
        {
            if (bg.ThoiGian.Overlaps(newPeriod))
            {
                // Auto-expire previous open-ended price list if the new one starts after its start date
                if (bg.ThoiGian.NgayKetThuc == null && bg.ThoiGian.NgayBatDau < ngayApDung)
                {
                    bg.ExpireAt(ngayApDung.AddTicks(-1));
                }
                else
                {
                    throw new BusinessException($"Bảng giá '{bg.TenBangGia}' đang có thời gian hiệu lực chồng lấn với bảng giá mới.");
                }
            }
        }
    }

    public BangGia? GetCurrentPrice(DateTime atDate)
    {
        return _bangGias
            .Where(bg => bg.IsActive && bg.ThoiGian.IsActive(atDate))
            .OrderByDescending(bg => bg.ThoiGian.NgayBatDau)
            .FirstOrDefault();
    }

    public void ActivateBangGia(int id)
    {
        var bangGia = _bangGias.FirstOrDefault(x => x.Id == id);
        if (bangGia != null)
        {
            // Deactivate other overlapping BangGias if needed
            // Actually, BangGia.Activate() just sets IsActive = true.
            // But we should ensure no overlap for active ones.
            if (!bangGia.IsActive)
            {
                EnsureNoOverlap(bangGia.ThoiGian.NgayBatDau, bangGia.ThoiGian.NgayKetThuc);
                bangGia.Activate();
            }
        }
    }

    public void DeactivateBangGia(int id)
    {
        var bangGia = _bangGias.FirstOrDefault(x => x.Id == id);
        bangGia?.Deactivate();
    }

    public void RemoveBangGia(int id)
    {
        var bangGia = _bangGias.FirstOrDefault(x => x.Id == id);
        if (bangGia != null)
        {
            _bangGias.Remove(bangGia);
        }
    }

    public Result ActivateKhungGio(int id)
    {
        var khungGio = _khungGios.FirstOrDefault(x => x.Id == id);
        if (khungGio == null)
            return Result.Failure(DichVuErrors.KhungGioNotFound);

        if (!khungGio.IsActive)
        {
            if (_khungGios.Any(x => x.Id != id && x.OverlapsWith(khungGio.GioBatDau, khungGio.GioKetThuc, khungGio.NgayTrongTuan)))
            {
                return Result.Failure(DichVuErrors.KhungGioOverlap);
            }
            khungGio.Activate();
        }

        return Result.Success();
    }

    public void DeactivateKhungGio(int id)
    {
        var khungGio = _khungGios.FirstOrDefault(x => x.Id == id);
        khungGio?.Deactivate();
    }

    public void RemoveKhungGio(int id)
    {
        var khungGio = _khungGios.FirstOrDefault(x => x.Id == id);
        if (khungGio != null)
        {
            _khungGios.Remove(khungGio);
        }
    }
}
