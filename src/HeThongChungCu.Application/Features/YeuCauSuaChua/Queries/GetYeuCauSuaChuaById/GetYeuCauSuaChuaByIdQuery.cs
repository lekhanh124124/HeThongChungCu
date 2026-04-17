using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;

public record GetYeuCauSuaChuaByIdQuery(int Id) : IQuery<YeuCauSuaChuaDetailResponse>;
