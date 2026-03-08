namespace HeThongChungCu.Application.Features.ChungCu.DTOs;

public class CanHoResponse
{
    public int Id { get; set; }
    public int ToaNhaId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public decimal DienTich { get; set; }
    public int Tang { get; set; }
    public int SoPhongNgu { get; set; }
    public int SoPhongTam { get; set; }
    public int TinhTrangCanHoId { get; set; }
}
