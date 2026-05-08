using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKetQuaKhaoSat;

public record GetKetQuaKhaoSatQuery(int Id) : IQuery<KetQuaKhaoSatResponse>;
