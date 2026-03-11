using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.DeleteTang;

public record DeleteTangCommand(IReadOnlyList<int> Ids) : ICommand<IReadOnlyList<TangDetailResponse>>;
