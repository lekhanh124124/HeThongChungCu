using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.RevokeDichVu;

public record RevokeDichVuCommand(List<int> Ids) : ICommand<bool>;
