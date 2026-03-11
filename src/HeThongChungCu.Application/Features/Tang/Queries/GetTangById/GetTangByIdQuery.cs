using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Queries.GetTangById;

public record GetTangByIdQuery(int Id) : IQuery<TangResponse>;
