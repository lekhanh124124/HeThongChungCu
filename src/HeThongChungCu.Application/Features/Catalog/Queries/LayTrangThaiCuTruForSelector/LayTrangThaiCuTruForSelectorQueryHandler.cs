using HeThongChungCu.Application.Features.Catalog.DTOs;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayTrangThaiCuTruForSelector;

public class LayTrangThaiCuTruForSelectorQueryHandler : IQueryHandler<LayTrangThaiCuTruForSelectorQuery, IReadOnlyList<ItemForSelectorResponse>>
{
    public Task<Result<IReadOnlyList<ItemForSelectorResponse>>> Handle(LayTrangThaiCuTruForSelectorQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<ItemForSelectorResponse> result = TrangThaiCuTru.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return Task.FromResult(Result.Success(result));
    }
}
