using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayGioiTinhForSelector;

public class LayGioiTinhForSelectorQueryHandler : IQueryHandler<LayGioiTinhForSelectorQuery, IReadOnlyList<ItemForSelectorResponse>>
{
    public Task<Result<IReadOnlyList<ItemForSelectorResponse>>> Handle(LayGioiTinhForSelectorQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<ItemForSelectorResponse> result = GioiTinh.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return Task.FromResult(Result.Success(result));
    }
}
