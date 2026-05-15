using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietQuyThuChi : AuditableEntity
{
    public int QuyThuChiId { get; private set; }
    public decimal SoTien { get; private set; }
    
    public int? DichVuId { get; private set; }
    public string NhomThongKe { get; private set; } = null!;
    public string? GhiChu { get; private set; }

    private ChiTietQuyThuChi() { } // For EF Core

    internal ChiTietQuyThuChi(decimal soTien, string nhomThongKe, string? ghiChu, int? dichVuId = null)
    {
        SoTien = soTien;
        NhomThongKe = nhomThongKe;
        GhiChu = ghiChu;
        DichVuId = dichVuId;
    }
}
