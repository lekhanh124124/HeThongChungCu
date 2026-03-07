using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.DeleteToaNha;

public record DeleteToaNhaCommand(
    IReadOnlyList<int> Ids) : ICommand<IReadOnlyList<ToaNhaResponse>>;
