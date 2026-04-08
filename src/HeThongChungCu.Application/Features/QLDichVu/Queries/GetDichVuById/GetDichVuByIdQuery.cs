using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetDichVuById;

public record GetDichVuByIdQuery(int Id) : IQuery<DichVuDetailResponse>;
