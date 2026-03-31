using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;

public class LayDSYeuCauCuTruQueryHandler : IQueryHandler<LayDSYeuCauCuTruQuery, PagedResult<DSYeuCauCuTruResponse>>
{
    private readonly IYeuCauCuTruDapperRepository _dapperRepository;

    public LayDSYeuCauCuTruQueryHandler(IYeuCauCuTruDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public async Task<Result<PagedResult<DSYeuCauCuTruResponse>>> Handle(LayDSYeuCauCuTruQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayDSYeuCauCuTruQuerySpecification(
            request.ToaNhaId,
            request.TangId,
            request.CanHoId,
            request.LoaiYeuCauId,
            request.TrangThaiId,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _dapperRepository.GetPagedListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
