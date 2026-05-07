using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;

public record GetHangMucBaoTriByIdQuery(int Id) : IQuery<HangMucBaoTriDetailResponse>;
