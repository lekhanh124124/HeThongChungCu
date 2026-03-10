using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.DeleteToaNha;

public record DeleteToaNhaCommand(
    IReadOnlyList<int> Ids) : ICommand<IReadOnlyList<ToaNhaDetailResponse>>;
