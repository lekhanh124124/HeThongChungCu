using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.LapHoaDonDuThao;

public record LapHoaDonDuThaoCommand : ICommand<LapHoaDonDuThaoResponse>
{
    public int DotThanhToanId { get; init; }
}
