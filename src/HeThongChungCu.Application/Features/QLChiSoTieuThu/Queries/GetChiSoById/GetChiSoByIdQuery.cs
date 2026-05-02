using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetChiSoById;

public record GetChiSoByIdQuery(int Id) : IQuery<ChiSoDetailResponse>;
