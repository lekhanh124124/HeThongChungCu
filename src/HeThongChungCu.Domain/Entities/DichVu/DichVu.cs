using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class DichVu : AggregateRoot
{
    public int? DoiTacId { get; private set; }
    public string MaDichVu { get; private set; } = string.Empty;
    public string TenDichVu { get; private set; } = string.Empty;
    public LoaiDichVu LoaiDichVuId { get; private set; } = null!;
    public string DonViTinh { get; private set; } = string.Empty;
    public string? MoTa { get; private set; }
    public bool IsBatBuoc { get; private set; }
    public bool IsActive { get; private set; }
    public int? IconId { get; private set; }
    public TepTaiLieu? Icon { get; private set; }

    private readonly List<BangGia> _bangGias = [];
    public IReadOnlyCollection<BangGia> BangGias => _bangGias.AsReadOnly();

    private DichVu() { } // EF Core

    public DichVu(
        string maDichVu,
        string tenDichVu,
        LoaiDichVu loaiDichVuId,
        string donViTinh,
        string? moTa = null,
        int? iconId = null,
        int? doiTacId = null,
        bool isBatBuoc = false)
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
        DoiTacId = doiTacId;
        IsBatBuoc = isBatBuoc;
        IsActive = true;
    }

    public void Update(
        string tenDichVu,
        LoaiDichVu loaiDichVuId,
        string donViTinh,
        string? moTa,
        int? iconId,
        int? doiTacId,
        bool isBatBuoc)
    {
        TenDichVu = tenDichVu;
        LoaiDichVuId = loaiDichVuId;
        DonViTinh = donViTinh;
        MoTa = moTa;
        IconId = iconId;
        DoiTacId = doiTacId;
        IsBatBuoc = isBatBuoc;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    // --- Price Management ---
    public void AddBangGia(
        string tenBangGia,
        DateTime ngayApDung,
        LoaiDinhGia loaiDinhGiaId,
        decimal donGia = 0)
    {
        // Check for overlaps with existing active price lists
        if (_bangGias.Any(bg => bg.IsActive && bg.IsOverlapping(ngayApDung, null)))
        {
            throw new BusinessException("Thời gian áp dụng bảng giá mới bị chồng lấn với bảng giá hiện tại.");
        }

        // Close last active price list if needed (optional logic, usually you want them to be sequential)
        var lastActive = _bangGias.OrderByDescending(bg => bg.ThoiGian.NgayBatDau).FirstOrDefault(bg => bg.IsActive);

        var newBangGia = new BangGia(Id, tenBangGia, ngayApDung, loaiDinhGiaId, donGia);
        _bangGias.Add(newBangGia);
    }

    public BangGia? GetCurrentPrice(DateTime atDate)
    {
        return _bangGias
            .Where(bg => bg.IsActive && bg.ThoiGian.IsActive(atDate))
            .OrderByDescending(bg => bg.ThoiGian.NgayBatDau)
            .FirstOrDefault();
    }
}
