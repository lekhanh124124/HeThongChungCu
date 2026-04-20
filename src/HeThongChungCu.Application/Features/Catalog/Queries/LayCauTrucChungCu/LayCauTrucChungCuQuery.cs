using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;

public record LayCauTrucChungCuQuery(string? Keyword = null) : IQuery<List<CauTrucToaNhaResponse>>;
