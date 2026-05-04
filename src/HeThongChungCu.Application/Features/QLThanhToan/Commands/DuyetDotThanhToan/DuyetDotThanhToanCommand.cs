using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DuyetDotThanhToan;

public record DuyetDotThanhToanCommand(List<int> Ids) : ICommand<bool>;
