using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteDoiTac;

public record DeleteDoiTacsCommand(List<int> Ids) : ICommand<IReadOnlyList<DoiTacResponse>>;
