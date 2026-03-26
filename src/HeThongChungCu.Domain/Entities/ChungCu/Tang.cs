using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class Tang : AuditableEntity
{
    public string MaTang { get; internal set; } = null!;
    public string TenTang { get; internal set; } = null!;
    public LoaiTang LoaiTangId { get; internal set; } = null!;

    public int ToaNhaId { get; private set; }
    public ToaNha ToaNha { get; private set; } = null!;

    private Tang() { } // EF Core

    internal Tang(string maTang, string tenTang, LoaiTang loaiTangId, int toaNhaId)
    {
        MaTang = maTang;
        TenTang = tenTang;
        LoaiTangId = loaiTangId;
        ToaNhaId = toaNhaId;
    }
}
