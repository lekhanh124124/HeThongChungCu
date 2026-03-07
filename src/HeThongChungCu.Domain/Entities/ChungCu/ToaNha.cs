using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class ToaNha : AggregateRoot
{
    public string MaToaNha { get; private set; } = null!;
    public string TenToaNha { get; private set; } = null!;
    public int SoTang { get; private set; }

    private readonly List<CanHo> _canHos = new();
    public IReadOnlyCollection<CanHo> CanHos => _canHos.AsReadOnly();

    private ToaNha() { } // EF Core

    public ToaNha(string maToaNha, string tenToaNha, int soTang)
    {
        MaToaNha = maToaNha;
        TenToaNha = tenToaNha;
        SoTang = soTang;
    }

    public void Update(string tenToaNha, int soTang)
    {
        TenToaNha = tenToaNha;
        SoTang = soTang;
    }

    public void AddCanHo(CanHo canHo)
    {
        _canHos.Add(canHo);
    }
}
