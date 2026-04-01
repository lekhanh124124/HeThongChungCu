using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Commands.DeleteCanHo;

public record DeleteCanHoCommand(
    IReadOnlyList<int> Ids) : ICommand<IReadOnlyList<CanHoDetailResponse>>;
