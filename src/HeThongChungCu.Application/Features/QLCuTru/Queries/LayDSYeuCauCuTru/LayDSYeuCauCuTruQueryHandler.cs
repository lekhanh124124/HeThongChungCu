using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;

public class LayDSYeuCauCuTruQueryHandler : IQueryHandler<LayDSYeuCauCuTruQuery, PagedResult<DSYeuCauCuTruResponse>>
{
    private readonly IYeuCauCuTruQueryRepository _QueryRepository;

    public LayDSYeuCauCuTruQueryHandler(IYeuCauCuTruQueryRepository QueryRepository)
    {
        _QueryRepository = QueryRepository;
    }

    public async Task<Result<PagedResult<DSYeuCauCuTruResponse>>> Handle(LayDSYeuCauCuTruQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayDSYeuCauCuTruQuerySpecification(
            request.ToaNhaId,
            request.TangId,
            request.CanHoId,
            request.LoaiYeuCauId,
            request.TrangThaiId,
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _QueryRepository.GetPagedListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
