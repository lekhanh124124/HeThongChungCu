using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteDichVu;

public record DeleteDichVuCommand(List<int> Ids) : ICommand<bool>;
