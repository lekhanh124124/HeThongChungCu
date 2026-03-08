using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.DeleteCanHo;

public record DeleteCanHoCommand(
    IReadOnlyList<int> Ids) : ICommand<IReadOnlyList<CanHoResponse>>;
