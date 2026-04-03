using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

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
        ValidateStructure(dienTich, soPhongNgu, soPhongTam);

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
        LoaiCanHo loaiCanHoId)
    {
        ValidateStructure(dienTich, soPhongNgu, soPhongTam);

        MaCanHo = maCanHo;
        TenCanHo = tenCanHo;
        DienTich = dienTich;
        SoPhongNgu = soPhongNgu;
        SoPhongTam = soPhongTam;
        LoaiCanHoId = loaiCanHoId;
    }

    public void UpdateStatus(TrangThaiCanHo nextStatus)
    {
        if (TinhTrangCanHoId == nextStatus) return;

        // Rule: ChuaBanGiao -> DangTrong -> CoCuDan
        if (TinhTrangCanHoId == TrangThaiCanHo.ChuaBanGiao && nextStatus == TrangThaiCanHo.CoCuDan)
        {
            throw new BusinessException("Không được chuyển trực tiếp từ 'Chưa bàn giao' sang 'Có cư dân'. Phải qua trạng thái 'Đang trống'.");
        }

        // Rule: CoCuDan -> DangTrong -> ChuaBanGiao
        if (TinhTrangCanHoId == TrangThaiCanHo.CoCuDan && nextStatus == TrangThaiCanHo.DangTrong)
        {
            throw new BusinessException("Không được chuyển trực tiếp từ 'Có cư dân' sang 'Đang trống'. Phải qua trạng thái 'Chưa bàn giao'.");
        }

        TinhTrangCanHoId = nextStatus;
    }

    public void Delete()
    {
    }

    private void ValidateStructure(decimal dienTich, int soPhongNgu, int soPhongTam)
    {
        if (dienTich <= 0)
            throw new BusinessException("Diện tích căn hộ phải lớn hơn 0.");

        if (soPhongNgu < 0)
            throw new BusinessException("Số phòng ngủ không được âm.");

        if (soPhongTam < 0)
            throw new BusinessException("Số phòng tắm không được âm.");
    }

}
