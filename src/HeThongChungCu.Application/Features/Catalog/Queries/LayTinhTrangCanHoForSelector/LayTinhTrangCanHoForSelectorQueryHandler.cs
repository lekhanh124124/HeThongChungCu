using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayTinhTrangCanHoForSelector;

public class LayTinhTrangCanHoForSelectorQueryHandler : IQueryHandler<LayTinhTrangCanHoForSelectorQuery, IReadOnlyList<ItemForSelectorResponse>>
{
    public Task<Result<IReadOnlyList<ItemForSelectorResponse>>> Handle(LayTinhTrangCanHoForSelectorQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<ItemForSelectorResponse> result = TrangThaiCanHo.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return Task.FromResult(Result.Success(result));
    }
}
