namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class HoaDonDetailResponse : HoaDonResponse
{
    public string? GhiChu { get; set; }
    public List<ChiTietHoaDonResponse> ChiTietHoaDons { get; set; } = [];
}
