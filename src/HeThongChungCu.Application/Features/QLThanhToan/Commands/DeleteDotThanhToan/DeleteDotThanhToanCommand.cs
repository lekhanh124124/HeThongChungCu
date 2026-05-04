using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DeleteDotThanhToan;

public record DeleteDotThanhToanCommand(List<int> Ids) : ICommand<bool>;
