using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateDichVu;

public record ActivateDichVuCommand(List<int> Ids) : ICommand<bool>;
