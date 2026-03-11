using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class Tang : AuditableEntity
{
    public string MaTang { get; private set; } = null!;
    public string TenTang { get; private set; } = null!;
    public int LoaiTangId { get; private set; }
    public int ToaNhaId { get; private set; }

    // Navigation properties
    public ToaNha ToaNha { get; private set; } = null!;
    private readonly List<CanHo> _canHos = new();
    public IReadOnlyCollection<CanHo> CanHos => _canHos.AsReadOnly();

    private Tang() { } // EF Core

    public Tang(string maTang, string tenTang, int loaiTangId, int toaNhaId)
    {
        MaTang = maTang;
        TenTang = tenTang;
        LoaiTangId = loaiTangId;
        ToaNhaId = toaNhaId;
    }

    public void Update(string maTang, string tenTang, int loaiTangId)
    {
        MaTang = maTang;
        TenTang = tenTang;
        LoaiTangId = loaiTangId;
    }

    public void AddCanHo(CanHo canHo)
    {
        _canHos.Add(canHo);
    }
}
