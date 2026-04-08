using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListKhungGioDichVu;

public class GetListKhungGioDichVuQueryHandler : IQueryHandler<GetListKhungGioDichVuQuery, PagedResult<KhungGioDichVuResponse>>
{
    private readonly IDichVuQueryRepository _queryRepository;

    public GetListKhungGioDichVuQueryHandler(IDichVuQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<KhungGioDichVuResponse>>> Handle(GetListKhungGioDichVuQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListKhungGioDichVuSpecification(
            request.DichVuId,
            request.Keyword,
            request.PageNumber,
            request.PageSize,
            request.SortCol,
            request.IsAsc);

        var result = await _queryRepository.GetListKhungGioAsync(spec, cancellationToken);
        return Result<PagedResult<KhungGioDichVuResponse>>.Success(result);
    }
}
