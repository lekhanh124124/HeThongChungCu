using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;

public class LayDSYeuCauPhuongTienQueryHandler : IQueryHandler<LayDSYeuCauPhuongTienQuery, PagedResult<DSYeuCauPhuongTienResponse>>
{
    private readonly IYeuCauPhuongTienQueryRepository _QueryRepository;

    public LayDSYeuCauPhuongTienQueryHandler(IYeuCauPhuongTienQueryRepository QueryRepository)
    {
        _QueryRepository = QueryRepository;
    }

    public async Task<Result<PagedResult<DSYeuCauPhuongTienResponse>>> Handle(LayDSYeuCauPhuongTienQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayDSYeuCauPhuongTienQuerySpecification(
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
