using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiById;

public record GetThietBiByIdQuery(int Id) : IQuery<ThietBiDetailResponse>;
