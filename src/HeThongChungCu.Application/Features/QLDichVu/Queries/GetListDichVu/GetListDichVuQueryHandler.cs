using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListDichVu;

public class GetListDichVuQueryHandler : IQueryHandler<GetListDichVuQuery, PagedResult<DichVuResponse>>
{
    private readonly IDichVuQueryRepository _dichVuQueryRepository;

    public GetListDichVuQueryHandler(IDichVuQueryRepository dichVuQueryRepository)
    {
        _dichVuQueryRepository = dichVuQueryRepository;
    }

    public async Task<Result<PagedResult<DichVuResponse>>> Handle(GetListDichVuQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListDichVuSpecification(
            request.LoaiDichVuId,
            request.DoiTacId,
            request.HopDongDoiTacId,
            request.Keyword,
            request.PageNumber,
            request.PageSize,
            request.SortCol,
            request.IsAsc);

        var result = await _dichVuQueryRepository.GetListAsync(spec, cancellationToken);

        return Result.Success(result);
    }
}
