using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListBangGia;

public class GetListBangGiaQueryHandler : IQueryHandler<GetListBangGiaQuery, PagedResult<BangGiaResponse>>
{
    private readonly IDichVuQueryRepository _queryRepository;

    public GetListBangGiaQueryHandler(IDichVuQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<BangGiaResponse>>> Handle(GetListBangGiaQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListBangGiaSpecification(
            request.DichVuId,
            request.Keyword,
            request.IsActive,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.IsAsc);

        return await _queryRepository.GetListBangGiaAsync(spec, cancellationToken);
    }
}
