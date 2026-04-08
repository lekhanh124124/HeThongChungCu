using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetKhungGioDichVuById;

public record GetKhungGioDichVuByIdQuery(int Id) : IQuery<KhungGioDichVuResponse>;
