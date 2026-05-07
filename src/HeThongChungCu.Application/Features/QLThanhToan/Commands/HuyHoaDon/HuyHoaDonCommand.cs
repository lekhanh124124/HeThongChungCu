using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.HuyHoaDon;

public record HuyHoaDonCommand(int HoaDonId, string? LyDo) : ICommand<bool>;
