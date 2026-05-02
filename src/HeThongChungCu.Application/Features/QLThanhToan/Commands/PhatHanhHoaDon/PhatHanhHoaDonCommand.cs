using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.PhatHanhHoaDon;

public record PhatHanhHoaDonCommand : ICommand<bool>
{
    public int DotThanhToanId { get; init; }
    public List<int>? HoaDonIds { get; init; }
}
