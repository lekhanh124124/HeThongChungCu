using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class HangMucBaoTri : AuditableEntity
{
    public string MaHangMuc { get; private set; } = null!;
    public string TenHangMuc { get; private set; } = null!;
    public string? MoTa { get; private set; }
    public int ThoiGianUocTinhPhut { get; private set; }
    public decimal ChiPhiUocTinh { get; private set; }
    public string ChecklistTieuChuan { get; private set; } = null!; // JSON Array string e.g. ["Kiểm tra cáp", "Kiểm tra động cơ"]

    private HangMucBaoTri() : base() { } // EF Core

    private HangMucBaoTri(
        string maHangMuc,
        string tenHangMuc,
        string? moTa,
        int thoiGianUocTinhPhut,
        decimal chiPhiUocTinh,
        string checklistTieuChuan) : base()
    {
        MaHangMuc = maHangMuc;
        TenHangMuc = tenHangMuc;
        MoTa = moTa;
        ThoiGianUocTinhPhut = thoiGianUocTinhPhut;
        ChiPhiUocTinh = chiPhiUocTinh;
        ChecklistTieuChuan = checklistTieuChuan;
    }

    public static HangMucBaoTri Create(
        string maHangMuc,
        string tenHangMuc,
        string? moTa,
        int thoiGianUocTinhPhut,
        decimal chiPhiUocTinh,
        string checklistTieuChuan)
    {
        return new HangMucBaoTri(
            maHangMuc,
            tenHangMuc,
            moTa,
            thoiGianUocTinhPhut,
            chiPhiUocTinh,
            checklistTieuChuan);
    }

    public void Update(
        string tenHangMuc,
        string? moTa,
        int thoiGianUocTinhPhut,
        decimal chiPhiUocTinh,
        string checklistTieuChuan)
    {
        TenHangMuc = tenHangMuc;
        MoTa = moTa;
        ThoiGianUocTinhPhut = thoiGianUocTinhPhut;
        ChiPhiUocTinh = chiPhiUocTinh;
        ChecklistTieuChuan = checklistTieuChuan;
    }
}
