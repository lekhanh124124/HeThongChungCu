using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayDichVuForSelector;

public class LayDichVuForSelectorQueryHandler : IQueryHandler<LayDichVuForSelectorQuery, IReadOnlyList<ItemForSelectorResponse>>
{
    private readonly IDichVuCommandRepository _repository;

    public LayDichVuForSelectorQueryHandler(IDichVuCommandRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<ItemForSelectorResponse>>> Handle(LayDichVuForSelectorQuery request, CancellationToken cancellationToken)
    {
        var services = await _repository.GetAllAsync(cancellationToken);
        
        var result = services
            .Where(x => x.IsActive)
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Id,
                Name = x.TenDichVu
            })
            .ToList();

        return Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result);
    }
}
