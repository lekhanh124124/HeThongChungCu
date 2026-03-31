using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;

public class LayDSYeuCauPhuongTienQueryHandler : IQueryHandler<LayDSYeuCauPhuongTienQuery, PagedResult<DSYeuCauPhuongTienResponse>>
{
    private readonly IYeuCauPhuongTienDapperRepository _dapperRepository;

    public LayDSYeuCauPhuongTienQueryHandler(IYeuCauPhuongTienDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public async Task<Result<PagedResult<DSYeuCauPhuongTienResponse>>> Handle(LayDSYeuCauPhuongTienQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayDSYeuCauPhuongTienQuerySpecification(
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
