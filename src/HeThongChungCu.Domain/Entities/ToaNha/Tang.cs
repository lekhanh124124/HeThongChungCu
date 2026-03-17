using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class Tang : AuditableEntity
{
    public string MaTang { get; private set; } = null!;
    public string TenTang { get; private set; } = null!;
    public LoaiTang LoaiTangId { get; private set; } = null!;

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

    public void Update(string maTang, string tenTang, LoaiTang loaiTangId)
    {
        MaTang = maTang;
        TenTang = tenTang;
        LoaiTangId = loaiTangId;
    }

}
