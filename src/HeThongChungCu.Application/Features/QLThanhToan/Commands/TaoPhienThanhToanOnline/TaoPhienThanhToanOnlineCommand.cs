using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.TaoPhienThanhToanOnline;

public record TaoPhienThanhToanOnlineResponse(
    string MaThanhToan,
    decimal SoTien,
    string VietQrUrl
);

public record TaoPhienThanhToanOnlineCommand : ICommand<TaoPhienThanhToanOnlineResponse>
{
    public int HoaDonId { get; init; }
    public List<int> ChiTietHoaDonIds { get; init; } = new();
}
