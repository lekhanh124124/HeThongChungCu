using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Queries.GetToaNhaById;

public record GetToaNhaByIdQuery(int Id) : IQuery<ToaNhaDetailResponse>;
