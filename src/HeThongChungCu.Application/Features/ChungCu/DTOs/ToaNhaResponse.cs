namespace HeThongChungCu.Application.Features.ChungCu.DTOs;

public class ToaNhaResponse
{
    public int Id { get; set; }
    public string MaToaNha { get; set; } = null!;
    public string TenToaNha { get; set; } = null!;
    public int SoTang { get; set; }
}
