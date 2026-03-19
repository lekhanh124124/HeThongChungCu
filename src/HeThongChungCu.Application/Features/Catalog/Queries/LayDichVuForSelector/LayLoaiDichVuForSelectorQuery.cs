using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayDichVuForSelector;

public record LayDichVuForSelectorQuery : IQuery<IReadOnlyList<ItemForSelectorResponse>>;
