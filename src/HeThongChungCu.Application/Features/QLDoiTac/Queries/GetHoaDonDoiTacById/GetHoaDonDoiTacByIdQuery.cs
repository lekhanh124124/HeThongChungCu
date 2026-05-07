using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetHoaDonDoiTacById;

public record GetHoaDonDoiTacByIdQuery(int Id) : IQuery<HoaDonDoiTacDetailResponse>;
