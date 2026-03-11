namespace HeThongChungCu.Application.Features.Catalog.DTOs;

public class CauTrucCanHoResponse
{
    public int Id { get; set; }
    public string MaCanHo { get; set; } = null!;
    public string TenCanHo { get; set; } = null!;
    public int TrangThaiId { get; set; }
    public string TenTrangThai { get; set; } = null!;
}
