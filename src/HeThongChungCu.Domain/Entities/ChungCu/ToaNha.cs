using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;


namespace HeThongChungCu.Domain.Entities;

public class ToaNha : AggregateRoot
{
    public string MaToaNha { get; private set; } = null!;
    public string TenToaNha { get; private set; } = null!;

    public DiaChi DiaChi { get; private set; } = null!;
    public string? MoTa { get; private set; }
    public TrangThaiToaNha TrangThaiToaNhaId { get; private set; } = default!;

    private readonly List<Tang> _tangs = new();
    public IReadOnlyCollection<Tang> Tangs => _tangs.AsReadOnly();


    private ToaNha() { } // EF Core

    public ToaNha(
        string maToaNha, 
        string tenToaNha, 
        string? diaChi, 
        string? moTa, 
        TrangThaiToaNha trangThaiToaNhaId)
    {
        MaToaNha = maToaNha;
        TenToaNha = tenToaNha;
        DiaChi = new DiaChi(diaChi);
        MoTa = moTa;
        TrangThaiToaNhaId = trangThaiToaNhaId;
    }

    public void Update(
        string maToaNha,
        string tenToaNha, 
        string? diaChi, 
        string? moTa, 
        TrangThaiToaNha? trangThaiToaNhaId)
    {
        TenToaNha = tenToaNha;
        DiaChi = new DiaChi(diaChi);
        MoTa = moTa;
        TrangThaiToaNhaId = trangThaiToaNhaId ?? TrangThaiToaNhaId;
    }

    public Tang AddTang(string maTang, string tenTang, LoaiTang loaiTangId)
    {
        if (_tangs.Any(x => x.MaTang == maTang))
            throw new BusinessException("Mã tầng đã tồn tại.");

        if (TrangThaiToaNhaId != TrangThaiToaNha.DangHoatDong)
            throw new BusinessException("Tòa nhà chưa được hoạt động.");

        var tang = new Tang(maTang, tenTang, loaiTangId, Id);
        _tangs.Add(tang);

        return tang;
    }

    public void UpdateTang(int tangId, string maTang, string tenTang, LoaiTang loaiTangId)
    {
        var tang = _tangs.FirstOrDefault(x => x.Id == tangId);
        if (tang == null)
            throw new BusinessException("Không tìm thấy tầng.");

        if (_tangs.Any(x => x.MaTang == maTang && x.Id != tangId))
            throw new BusinessException("Mã tầng đã tồn tại.");

        tang.MaTang = maTang;
        tang.TenTang = tenTang;
        tang.LoaiTangId = loaiTangId;
    }
}
