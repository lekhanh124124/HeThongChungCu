namespace HeThongChungCu.Application.Features.QLTaiChinh.DTOs;

public class BaoCaoCongNoToaNhaResponse
{
    public int ToaNhaId { get; set; }
    public string TenToaNha { get; set; } = null!;
    public int TongSoCanHo { get; set; }
    public int SoCanHoNoPhi { get; set; }
    public decimal TongNoDauKy { get; set; }
    public decimal TongPhatSinh { get; set; }
    public decimal TongDaThu { get; set; }
    public decimal TongNoConLai { get; set; }
    public double TyLeThuHoi { get; set; }
}
