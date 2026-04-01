using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetToaNhaById;

public record GetToaNhaByIdQuery(int Id) : IQuery<ToaNhaResponse>;
