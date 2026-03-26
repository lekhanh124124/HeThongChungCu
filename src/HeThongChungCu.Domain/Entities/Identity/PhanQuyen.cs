using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class PhanQuyen : BaseEntity
{
    public int TaiKhoanId { get; private set; }
    public TaiKhoan TaiKhoan { get; private set; } = null!;
    public Role RoleId { get; private set; } = null!;

    private PhanQuyen() { } // EF Core

    internal PhanQuyen(int taiKhoanId, Role role)
    {
        TaiKhoanId = taiKhoanId;
        RoleId = role;
    }
}
