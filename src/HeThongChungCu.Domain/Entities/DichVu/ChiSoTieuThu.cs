using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class ChiSoTieuThu : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int DichVuId { get; private set; }
    public decimal ChiSoCu { get; private set; }
    public decimal ChiSoMoi { get; private set; }
    public decimal SoLuong => ChiSoMoi - ChiSoCu;
    public int Thang { get; private set; }
    public int Nam { get; private set; }
    public DateTimeOffset NgayGhiNhan { get; private set; }
    public TrangThaiChiSo TrangThaiChiSoId { get; private set; } = null!;
    public int? AnhDongHoId { get; private set; }
    public string? GhiChu { get; private set; }
    public int? HoaDonId { get; private set; }
    public string? MaTraCuu { get; private set; }

    public void MarkAsBilled(int hoaDonId)
    {
        if (TrangThaiChiSoId != TrangThaiChiSo.Confirmed && TrangThaiChiSoId != TrangThaiChiSo.Locked)
            throw new BusinessException("Chỉ số chưa xác nhận không thể lập hóa đơn.");
        
        HoaDonId = hoaDonId;
        TrangThaiChiSoId = TrangThaiChiSo.Locked;
    }

    private ChiSoTieuThu() { } // EF Core

    private ChiSoTieuThu(
        int canHoId, 
        int dichVuId, 
        decimal chiSoCu, 
        decimal chiSoMoi, 
        int thang, 
        int nam, 
        DateTimeOffset ngayGhiNhan,
        int? anhDongHoId = null,
        string? ghiChu = null,
        string? maTraCuu = null)
    {
        CanHoId = canHoId;
        DichVuId = dichVuId;
        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        Thang = thang;
        Nam = nam;
        NgayGhiNhan = ngayGhiNhan;
        AnhDongHoId = anhDongHoId;
        GhiChu = ghiChu;
        MaTraCuu = maTraCuu;
        TrangThaiChiSoId = TrangThaiChiSo.Draft;
    }

    public static ChiSoTieuThu Create(
        int canHoId, 
        int dichVuId, 
        decimal chiSoCu, 
        decimal chiSoMoi, 
        int thang, 
        int nam, 
        DateTimeOffset ngayGhiNhan,
        int? anhDongHoId = null,
        string? ghiChu = null,
        string? maTraCuu = null)
    {
        if (chiSoMoi < chiSoCu)
            throw new BusinessException("Chỉ số mới không thể nhỏ hơn chỉ số cũ.");

        return new ChiSoTieuThu(
            canHoId, 
            dichVuId, 
            chiSoCu, 
            chiSoMoi, 
            thang, 
            nam, 
            ngayGhiNhan, 
            anhDongHoId, 
            ghiChu,
            maTraCuu);
    }

    public void Update(decimal chiSoCu, decimal chiSoMoi, int thang, int nam, DateTimeOffset ngayGhiNhan, int? anhDongHoId = null, string? ghiChu = null)
    {
        if (TrangThaiChiSoId == TrangThaiChiSo.Locked)
            throw new BusinessException("Chỉ số tiêu thụ đã lập hóa đơn, không thể cập nhật.");
        
        if (chiSoMoi < chiSoCu)
            throw new BusinessException("Chỉ số mới không thể nhỏ hơn chỉ số cũ.");

        ChiSoCu = chiSoCu;
        ChiSoMoi = chiSoMoi;
        Thang = thang;
        Nam = nam;
        NgayGhiNhan = ngayGhiNhan;
        AnhDongHoId = anhDongHoId;
        GhiChu = ghiChu;
    }

    public Result Confirm()
    {
        if (TrangThaiChiSoId == TrangThaiChiSo.Locked)
            return Result.Failure(new Error("ChiSo.Locked", "Chỉ số đã lập hóa đơn không thể chuyển trạng thái."));
        
        TrangThaiChiSoId = TrangThaiChiSo.Confirmed;
        return Result.Success();
    }

    public Result RevertToDraft()
    {
        if (TrangThaiChiSoId == TrangThaiChiSo.Locked)
            return Result.Failure(new Error("ChiSo.Locked", "Chỉ số đã lập hóa đơn không thể quay về bản nháp."));
        
        TrangThaiChiSoId = TrangThaiChiSo.Draft;
        return Result.Success();
    }

    public void SetAnhDongHo(int anhDongHoId)
    {
        if (TrangThaiChiSoId == TrangThaiChiSo.Locked)
            throw new BusinessException("Chỉ số đã lập hóa đơn, không thể cập nhật ảnh.");

        AnhDongHoId = anhDongHoId;
    }
}
