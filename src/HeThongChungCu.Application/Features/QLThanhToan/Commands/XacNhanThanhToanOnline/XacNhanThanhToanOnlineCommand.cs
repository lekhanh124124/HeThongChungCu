using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.XacNhanThanhToanOnline;

public record XacNhanThanhToanOnlineCommand : ICommand<bool>
{
    public string MaThanhToan { get; init; } = null!;
    public decimal SoTienThanhToan { get; init; } // Phục vụ đối soát
    public string? GiaoDichNganHangId { get; init; }
}
