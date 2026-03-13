using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class ToaNha : AggregateRoot
{
    public string MaToaNha { get; private set; } = null!;
    public string TenToaNha { get; private set; } = null!;

    public string DiaChi { get; private set; } = null!;
    public string? MoTa { get; private set; }
    public TrangThaiToaNha TrangThaiToaNhaId { get; private set; } = null!;

    private readonly List<Tang> _tangs = new();
    public IReadOnlyCollection<Tang> Tangs => _tangs.AsReadOnly();


    private ToaNha() { } // EF Core

    public ToaNha(string maToaNha, string tenToaNha, string diaChi, string? moTa, TrangThaiToaNha trangThaiToaNhaId)
    {
        MaToaNha = maToaNha;
        TenToaNha = tenToaNha;
        DiaChi = diaChi;
        MoTa = moTa;
        TrangThaiToaNhaId = trangThaiToaNhaId;
    }

    public void Update(string tenToaNha, string diaChi, string? moTa, TrangThaiToaNha? trangThaiToaNhaId)
    {
        TenToaNha = tenToaNha ?? TenToaNha;
        DiaChi = diaChi ?? DiaChi;
        MoTa = moTa ?? MoTa;
        TrangThaiToaNhaId = trangThaiToaNhaId ?? TrangThaiToaNhaId;
    }

    public void AddTang(Tang tang)
    {
        _tangs.Add(tang);
    }

}
