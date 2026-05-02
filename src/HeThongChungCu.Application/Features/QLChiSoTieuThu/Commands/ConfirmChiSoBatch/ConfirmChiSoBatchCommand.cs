using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ConfirmChiSoBatch;

public record ConfirmChiSoBatchCommand(List<int> ChiSoIds) : ICommand<int>;
