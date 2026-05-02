using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Models;

public class PaymentNotificationRequest
{
    public DotThanhToan DotThanhToan { get; set; } = null!;
    public List<HoaDon> HoaDons { get; set; } = new();
    public NguoiDung User { get; set; } = null!;
    public string? Email { get; set; }
}
