using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;

public record GetDoiTacByIdQuery(int Id) : IQuery<DoiTacDetailResponse>;
