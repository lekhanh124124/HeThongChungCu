using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhList;

public class GetPhanAnhListQueryHandler : IQueryHandler<GetPhanAnhListQuery, PagedResult<PhanAnhResponse>>
{
    private readonly IYeuCauPhanAnhQueryRepository _queryRepository;

    public GetPhanAnhListQueryHandler(IYeuCauPhanAnhQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<PhanAnhResponse>>> Handle(GetPhanAnhListQuery query, CancellationToken cancellationToken)
    {
        var spec = new GetPhanAnhListSpecification(
            query.CanHoId,
            query.TrangThaiPhanAnhId,
            query.LoaiPhanAnhId,
            query.NguoiXuLyId,
            query.Keyword,
            query.NgayTaoTu,
            query.NgayTaoDen,
            query.SortCol,
            query.IsAsc,
            query.PageNumber,
            query.PageSize);

        var result = await _queryRepository.GetAllAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
