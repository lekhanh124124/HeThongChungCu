using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;
using HeThongChungCu.Domain.Events;

namespace HeThongChungCu.Domain.Entities;

public class CanHo : AggregateRoot
{
    public string MaCanHo { get; private set; } = null!;
    public string TenCanHo { get; private set; } = null!;

    public ThongSoCanHo ThongSo { get; private set; } = null!;

    public LoaiCanHo LoaiCanHoId { get; private set; } = null!;
    public TrangThaiCanHo TinhTrangCanHoId { get; private set; } = null!;

    public int TangId { get; private set; }

    private CanHo() { } // EF Core

    public CanHo(
        int tangId,
        string maCanHo,
        string tenCanHo,
        decimal dienTich,
        int soPhongNgu,
        int soPhongTam,
        LoaiCanHo loaiCanHoId,
        TrangThaiCanHo tinhTrangCanHoId)
    {
        TangId = tangId;
        MaCanHo = maCanHo;
        TenCanHo = tenCanHo;
        ThongSo = new ThongSoCanHo(dienTich, soPhongNgu, soPhongTam);
        LoaiCanHoId = loaiCanHoId;
        TinhTrangCanHoId = tinhTrangCanHoId;
    }

    public static CanHo Create(
        int tangId,
        string maCanHo,
        string tenCanHo,
        decimal dienTich,
        int soPhongNgu,
        int soPhongTam,
        LoaiCanHo loaiCanHoId,
        TrangThaiCanHo tinhTrangCanHoId)
    {
        var canHo = new CanHo(
            tangId,
            maCanHo,
            tenCanHo,
            dienTich,
            soPhongNgu,
            soPhongTam,
            loaiCanHoId,
            tinhTrangCanHoId);

        canHo.AddDomainEvent(new CanHoCreatedDomainEvent(canHo));
        return canHo;
    }

    public void UpdateInfo(
        string tenCanHo,
        string maCanHo,
        decimal dienTich,
        int soPhongNgu,
        int soPhongTam,
        LoaiCanHo loaiCanHoId)
    {
        MaCanHo = maCanHo;
        TenCanHo = tenCanHo;
        ThongSo = new ThongSoCanHo(dienTich, soPhongNgu, soPhongTam);
        LoaiCanHoId = loaiCanHoId;
    }

    public void UpdateStatus(TrangThaiCanHo nextStatus)
    {
        if (TinhTrangCanHoId == nextStatus) return;

        // Rule: ChuaBanGiao -> CoCuDan (phải qua DangTrong trước)
        if (TinhTrangCanHoId == TrangThaiCanHo.ChuaBanGiao && nextStatus == TrangThaiCanHo.CoCuDan)
        {
            throw new BusinessException("Không được chuyển trực tiếp từ 'Chưa bàn giao' sang 'Có cư dân'. Phải qua trạng thái 'Đang trống'.");
        }

        TinhTrangCanHoId = nextStatus;
    }

    public void MarkAsOccupied()
    {
        if (TinhTrangCanHoId == TrangThaiCanHo.ChuaBanGiao)
        {
            UpdateStatus(TrangThaiCanHo.DangTrong);
        }
        UpdateStatus(TrangThaiCanHo.CoCuDan);
    }

    public void MarkAsVacant()
    {
        UpdateStatus(TrangThaiCanHo.DangTrong);
    }

    public void Delete()
    {
    }
}
