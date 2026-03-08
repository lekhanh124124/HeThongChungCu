using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Queries.GetCanHoById;

public record GetCanHoByIdQuery(int Id) : IQuery<CanHoDetailResponse>;
