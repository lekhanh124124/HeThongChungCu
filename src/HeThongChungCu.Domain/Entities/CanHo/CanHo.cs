using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.Policies;

namespace HeThongChungCu.Domain.Entities;

public class CanHo : AggregateRoot
{
    public string MaCanHo { get; private set; } = null!;
    public string TenCanHo { get; private set; } = null!;

    public decimal DienTich { get; private set; }
    public int SoPhongNgu { get; private set; }
    public int SoPhongTam { get; private set; }

    public LoaiCanHo LoaiCanHoId { get; private set; } = null!;
    public TrangThaiCanHo TinhTrangCanHoId { get; private set; } = null!;

    public int TangId { get; private set; }
    public global::HeThongChungCu.Domain.Entities.Tang Tang { get; private set; } = null!;

    private CanHo() { } // EF Core

    public CanHo(
        int tangId,
        string maCanHo, 
        string tenCanHo, 
        decimal dienTich, 
        int soPhongNgu, 
        int soPhongTam, 
        LoaiCanHo loaiCanHoId, 
        TrangThaiCanHo tinhTrangCanHoId,
        ICanHoPolicy policy)
    {
        policy.ValidateCreate(dienTich, soPhongNgu, soPhongTam);

        TangId = tangId;
        MaCanHo = maCanHo;
        TenCanHo = tenCanHo;
        DienTich = dienTich;
        SoPhongNgu = soPhongNgu;
        SoPhongTam = soPhongTam;
        LoaiCanHoId = loaiCanHoId;
        TinhTrangCanHoId = tinhTrangCanHoId;
    }

    public void UpdateInfo(
        string tenCanHo, 
        string maCanHo, 
        decimal dienTich, 
        int soPhongNgu, 
        int soPhongTam, 
        LoaiCanHo loaiCanHoId,
        ICanHoPolicy policy,
        bool hasActiveResidents)
    {
        policy.ValidateUpdate(this, dienTich, soPhongNgu, soPhongTam, loaiCanHoId, hasActiveResidents);

        MaCanHo = maCanHo;
        TenCanHo = tenCanHo;
        DienTich = dienTich;
        SoPhongNgu = soPhongNgu;
        SoPhongTam = soPhongTam;
        LoaiCanHoId = loaiCanHoId;
    }

    public void UpdateStatus(TrangThaiCanHo nextStatus, ICanHoPolicy policy)
    {
        policy.ValidateStatusChange(this, nextStatus);
        TinhTrangCanHoId = nextStatus;
    }

    public void Delete(ICanHoPolicy policy, bool hasActiveResidents)
    {
        policy.ValidateDelete(this, hasActiveResidents);
        // IsDeleted will be handled by the repository or a base class method if available
    }

}
