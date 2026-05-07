using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class LichBaoTri : AuditableEntity
{
    public int ThietBiId { get; private set; }
    public int HangMucBaoTriId { get; private set; }
    public TanSuatBaoTri TanSuatBaoTriId { get; private set; } = null!;
    public DateTimeOffset NgayBatDau { get; private set; }
    public DateTimeOffset? NgayKetThuc { get; private set; }
    public DateTimeOffset? NgayBaoTriGanNhat { get; private set; }
    public DateTimeOffset NgayBaoTriTiepTheo { get; private set; }
    public bool IsActive { get; private set; }

    private LichBaoTri() : base() { } // EF Core

    private LichBaoTri(
        int thietBiId,
        int hangMucBaoTriId,
        TanSuatBaoTri tanSuatBaoTriId,
        DateTimeOffset ngayBatDau,
        DateTimeOffset? ngayKetThuc,
        DateTimeOffset ngayBaoTriTiepTheo) : base()
    {
        ThietBiId = thietBiId;
        HangMucBaoTriId = hangMucBaoTriId;
        TanSuatBaoTriId = tanSuatBaoTriId;
        NgayBatDau = ngayBatDau;
        NgayKetThuc = ngayKetThuc;
        NgayBaoTriTiepTheo = ngayBaoTriTiepTheo;
        IsActive = true;
    }

    public static LichBaoTri Create(
        int thietBiId,
        int hangMucBaoTriId,
        TanSuatBaoTri tanSuatBaoTriId,
        DateTimeOffset ngayBatDau,
        DateTimeOffset? ngayKetThuc)
    {
        return new LichBaoTri(
            thietBiId,
            hangMucBaoTriId,
            tanSuatBaoTriId,
            ngayBatDau,
            ngayKetThuc,
            ngayBatDau); // Lần đầu tiên chạy chính là ngày bắt đầu
    }

    public void Update(
        TanSuatBaoTri tanSuatBaoTriId,
        DateTimeOffset ngayBatDau,
        DateTimeOffset? ngayKetThuc,
        bool isActive)
    {
        TanSuatBaoTriId = tanSuatBaoTriId;
        NgayBatDau = ngayBatDau;
        NgayKetThuc = ngayKetThuc;
        IsActive = isActive;
        NgayBaoTriTiepTheo = CalculateNextExecutionDate(NgayBaoTriGanNhat ?? ngayBatDau);
    }

    public void Toggle(bool isActive)
    {
        IsActive = isActive;
    }

    public void RecordExecution(DateTimeOffset executionDate)
    {
        NgayBaoTriGanNhat = executionDate;
        NgayBaoTriTiepTheo = CalculateNextExecutionDate(executionDate);
    }

    public DateTimeOffset CalculateNextExecutionDate(DateTimeOffset fromDate)
    {
        if (TanSuatBaoTriId == TanSuatBaoTri.HangNgay)
            return fromDate.AddDays(1);
        if (TanSuatBaoTriId == TanSuatBaoTri.HangTuan)
            return fromDate.AddDays(7);
        if (TanSuatBaoTriId == TanSuatBaoTri.HangThang)
            return fromDate.AddMonths(1);
        if (TanSuatBaoTriId == TanSuatBaoTri.HangQuy)
            return fromDate.AddMonths(3);
        if (TanSuatBaoTriId == TanSuatBaoTri.SauThang)
            return fromDate.AddMonths(6);
        if (TanSuatBaoTriId == TanSuatBaoTri.HangNam)
            return fromDate.AddYears(1);
 
        return fromDate.AddMonths(1); // Mặc định là hàng tháng nếu không khớp
    }
}
