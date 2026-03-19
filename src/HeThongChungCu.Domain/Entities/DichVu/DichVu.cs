using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class DichVu : AggregateRoot
{
    public string MaDichVu { get; private set; } = string.Empty;
    public string TenDichVu { get; private set; } = string.Empty;
    public string DonViTinh { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private DichVu() { } // EF Core

    public DichVu(
        string maDichVu,
        string tenDichVu,
        string donViTinh)
    {
        if (string.IsNullOrWhiteSpace(maDichVu))
            throw new BusinessException("Mã dịch vụ không được để trống.");
        if (string.IsNullOrWhiteSpace(tenDichVu))
            throw new BusinessException("Tên dịch vụ không được để trống.");

        MaDichVu = maDichVu;
        TenDichVu = tenDichVu;
        DonViTinh = donViTinh;
        IsActive = true;
    }

    public void Update(string tenDichVu, string donViTinh)
    {
        TenDichVu = tenDichVu;
        DonViTinh = donViTinh;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
