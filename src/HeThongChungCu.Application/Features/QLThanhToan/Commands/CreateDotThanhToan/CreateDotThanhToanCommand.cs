using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.CreateDotThanhToan;

public record CreateDotThanhToanCommand : ICommand<DotThanhToanDetailResponse>
{
    public int Thang { get; init; }
    public int Nam { get; init; }
}
