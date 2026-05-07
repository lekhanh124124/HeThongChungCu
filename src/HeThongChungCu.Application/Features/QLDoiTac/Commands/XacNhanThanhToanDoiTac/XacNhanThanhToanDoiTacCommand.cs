using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.XacNhanThanhToanDoiTac;

public record XacNhanThanhToanDoiTacCommand(int Id) : ICommand<bool>;
