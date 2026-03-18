using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayTrangThaiPhuongTienForSelector;

public class LayTrangThaiPhuongTienForSelectorQueryHandler : IQueryHandler<LayTrangThaiPhuongTienForSelectorQuery, IReadOnlyList<ItemForSelectorResponse>>
{
    public Task<Result<IReadOnlyList<ItemForSelectorResponse>>> Handle(LayTrangThaiPhuongTienForSelectorQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<ItemForSelectorResponse> result = TrangThaiPhuongTien.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return Task.FromResult(Result.Success(result));
    }
}
