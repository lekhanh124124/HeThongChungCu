using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.UpdateDotThanhToan;

public record UpdateDotThanhToanCommand : ICommand<DotThanhToanDetailResponse>
{
    public int Id { get; init; }
    public string TenDot { get; init; } = null!;
    public int Thang { get; init; }
    public int Nam { get; init; }
    public string? GhiChu { get; init; }
}
