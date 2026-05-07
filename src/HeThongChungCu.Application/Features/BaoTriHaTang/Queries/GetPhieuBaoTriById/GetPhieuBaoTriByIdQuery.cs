using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriById;

public record GetPhieuBaoTriByIdQuery(int Id) : IQuery<PhieuBaoTriDetailResponse>;
