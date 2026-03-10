namespace HeThongChungCu.Application.Features.CanHo.DTOs;

public class CanHoDetailResponse
{
    public int Id { get; set; }
    public int ToaNhaId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public int Tang { get; set; }
    public decimal DienTich { get; set; }
    public int SoPhongNgu { get; set; }
    public int SoPhongTam { get; set; }
    public int LoaiCanHoId { get; set; }
    public string TenLoaiCanHo { get; set; } = null!;
    public int TinhTrangCanHoId { get; set; }
    public string TenTinhTrangCanHo { get; set; } = null!;
}
