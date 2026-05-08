using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;

public record GetKhaoSatByIdQuery(int Id) : IQuery<KhaoSatDetailResponse>;
