using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DongDotThanhToan;

public record DongDotThanhToanCommand(int DotThanhToanId) : ICommand<bool>;
