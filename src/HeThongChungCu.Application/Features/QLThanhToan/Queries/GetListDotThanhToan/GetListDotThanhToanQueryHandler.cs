using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListDotThanhToan;

public class GetListDotThanhToanQueryHandler : IQueryHandler<GetListDotThanhToanQuery, PagedResult<DotThanhToanResponse>>
{
    private readonly IDotThanhToanQueryRepository _queryRepository;

    public GetListDotThanhToanQueryHandler(IDotThanhToanQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<DotThanhToanResponse>>> Handle(GetListDotThanhToanQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListDotThanhToanSpecification(
            request.Thang,
            request.Nam,
            request.TrangThaiId,
            request.Keyword,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.IsAsc);

        var result = await _queryRepository.GetListAsync(spec, cancellationToken);

        return Result.Success(result);
    }
}
