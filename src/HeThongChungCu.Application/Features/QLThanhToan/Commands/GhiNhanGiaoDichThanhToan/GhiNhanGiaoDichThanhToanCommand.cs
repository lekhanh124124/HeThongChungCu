using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.GhiNhanGiaoDichThanhToan;

public record GhiNhanGiaoDichThanhToanCommand : ICommand<List<int>>
{
    public int HoaDonId { get; init; }
    public List<int> ChiTietHoaDonIds { get; init; } = new();
    public int PhuongThucThanhToanId { get; init; }
    public string? MaGiaoDich { get; init; }
    public string? GhiChu { get; init; }
}
