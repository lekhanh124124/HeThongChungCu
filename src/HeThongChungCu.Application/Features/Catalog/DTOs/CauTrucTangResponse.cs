namespace HeThongChungCu.Application.Features.Catalog.DTOs;

public class CauTrucTangResponse
{
    public int Id { get; set; }
    public string MaTang { get; set; } = null!;
    public string TenTang { get; set; } = null!;

    public List<CauTrucCanHoResponse> CauTrucCanHos { get; set; } = new();
}
