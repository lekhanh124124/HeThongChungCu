using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayLoaiPhuongTienForSelector;

public class LayLoaiPhuongTienForSelectorQueryHandler : IQueryHandler<LayLoaiPhuongTienForSelectorQuery, IReadOnlyList<ItemForSelectorResponse>>
{
    public Task<Result<IReadOnlyList<ItemForSelectorResponse>>> Handle(LayLoaiPhuongTienForSelectorQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<ItemForSelectorResponse> result = LoaiPhuongTien.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return Task.FromResult(Result.Success(result));
    }
}
