using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class ToaNha : AggregateRoot
{
    public string MaToaNha { get; private set; } = null!;
    public string TenToaNha { get; private set; } = null!;
    public int SoTang { get; private set; }
    public int SoTangHam { get; private set; }

    public string DiaChi { get; private set; } = null!;
    public string? MoTa { get; private set; }
    public int TrangThaiToaNhaId { get; private set; }


    private readonly List<CanHo> _canHos = new();
    public IReadOnlyCollection<CanHo> CanHos => _canHos.AsReadOnly();

    private readonly List<ToaNhaHinhAnh> _hinhAnhs = new();
    public IReadOnlyCollection<ToaNhaHinhAnh> HinhAnhs => _hinhAnhs.AsReadOnly();

    private ToaNha() { } // EF Core

    public ToaNha(string maToaNha, string tenToaNha, int soTang, int soTangHam, string diaChi, string? moTa, int trangThaiToaNhaId)
    {
        MaToaNha = maToaNha;
        TenToaNha = tenToaNha;
        SoTang = soTang;
        SoTangHam = soTangHam;
        DiaChi = diaChi;
        MoTa = moTa;
        TrangThaiToaNhaId = trangThaiToaNhaId;
    }

    public void Update(string tenToaNha, int soTang, int soTangHam, string diaChi, string? moTa, int trangThaiToaNhaId)
    {
        TenToaNha = tenToaNha;
        SoTang = soTang;
        SoTangHam = soTangHam;
        DiaChi = diaChi;
        MoTa = moTa;
        TrangThaiToaNhaId = trangThaiToaNhaId;
    }

    public void AddCanHo(CanHo canHo)
    {
        _canHos.Add(canHo);
    }

    public void AddHinhAnh(ToaNhaHinhAnh hinhAnh)
    {
        _hinhAnhs.Add(hinhAnh);
    }
}
