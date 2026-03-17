using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

using HeThongChungCu.Domain.Policies;

namespace HeThongChungCu.Domain.Entities;

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

    public ToaNha(
        string maToaNha, 
        string tenToaNha, 
        string diaChi, 
        string? moTa, 
        TrangThaiToaNha trangThaiToaNhaId)
    {
        MaToaNha = maToaNha;
        TenToaNha = tenToaNha;
        DiaChi = diaChi;
        MoTa = moTa;
        TrangThaiToaNhaId = trangThaiToaNhaId;
    }

    public void Update(
        string maToaNha,
        string tenToaNha, 
        string diaChi, 
        string? moTa, 
        TrangThaiToaNha? trangThaiToaNhaId)
    {
        TenToaNha = tenToaNha;
        DiaChi = diaChi;
        MoTa = moTa;
        TrangThaiToaNhaId = trangThaiToaNhaId;
    }

    public void AddTang(string maTang, string tenTang, LoaiTang loaiTangId, IToaNhaPolicy policy)
    {
        policy.ValidateAddTang(maTang, this);

        var tang = new Tang(maTang, tenTang, loaiTangId, Id);
        _tangs.Add(tang);
    }

}
