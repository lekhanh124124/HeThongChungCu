using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

public record GetYeuCauCuTruByIdQuery(int RequestId) : IQuery<YeuCauCuTruResponse>;
