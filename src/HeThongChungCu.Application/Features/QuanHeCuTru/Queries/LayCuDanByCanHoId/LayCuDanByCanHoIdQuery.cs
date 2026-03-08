using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;

public record LayCuDanByCanHoIdQuery(int CanHoId) : IQuery<IReadOnlyList<CuDanResponse>>;
