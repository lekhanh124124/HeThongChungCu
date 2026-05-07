using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriById;

public record GetLichBaoTriByIdQuery(int Id) : IQuery<LichBaoTriDetailResponse>;
