namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public record LapHoaDonDuThaoResponse
{
    public int SoLuongHoaDonTaoMoi { get; init; }
    public int DotThanhToanId { get; init; }
    public string TenDotThanhToan { get; init; } = string.Empty;
}
