namespace HeThongChungCu.Application.Features.Catalog.DTOs;

public class CauTrucToaNhaResponse
{
    public int Id { get; set; }
    public string MaToaNha { get; set; } = null!;
    public string TenToaNha { get; set; } = null!;
    public int TrangThaiId { get; set; }
    public string TenTrangThai { get; set; } = null!;

    public IReadOnlyList<CauTrucTangResponse> CauTrucTangs { get; set; } = new List<CauTrucTangResponse>();
}