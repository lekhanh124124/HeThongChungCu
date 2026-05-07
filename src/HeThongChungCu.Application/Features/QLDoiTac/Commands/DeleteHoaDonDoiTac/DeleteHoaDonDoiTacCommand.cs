using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteHoaDonDoiTac;

public record DeleteHoaDonDoiTacCommand(int Id) : ICommand<bool>;
