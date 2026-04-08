namespace HeThongChungCu.Application.Features.QLDoiTac.DTOs;

public class DoiTacDetailResponse : DoiTacResponse
{
    public string? SoGiayPhepKD { get; set; }
    public string? MaSoThue { get; set; }
    public string? DiaChi { get; set; }
    public List<HopDongResponse> HopDongs { get; set; } = [];
}
