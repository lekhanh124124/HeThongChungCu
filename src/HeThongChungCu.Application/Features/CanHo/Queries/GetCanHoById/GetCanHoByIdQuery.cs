using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;

public record GetCanHoByIdQuery(int Id) : IQuery<CanHoResponse>;
