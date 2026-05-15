using System;

namespace HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

public class KhaoSatParticipantResponse
{
    public int CanHoId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public string TenChuHo { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public DateTimeOffset ThoiGianBieuQuyet { get; set; }
    public decimal TrongSo { get; set; }
    public bool IsOtpVerified { get; set; }
}
