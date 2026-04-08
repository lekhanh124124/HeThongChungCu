using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetBangGiaById;

public record GetBangGiaByIdQuery(int Id) : IQuery<BangGiaResponse?>;
