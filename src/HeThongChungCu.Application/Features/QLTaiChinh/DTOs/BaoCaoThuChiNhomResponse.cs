namespace HeThongChungCu.Application.Features.QLTaiChinh.DTOs;

public record BaoCaoThuChiNhomResponse
{
    public string NhomThongKe { get; init; } = string.Empty;
    public decimal TongSoTien { get; init; }
    public int SoGiaoDich { get; init; }
    public double TyLePhanTram { get; init; }
}
