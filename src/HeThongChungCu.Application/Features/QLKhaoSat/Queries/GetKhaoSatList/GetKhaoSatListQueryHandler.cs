using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Domain.Common;

using HeThongChungCu.Application.Common.Interfaces.Services;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatList;

public class GetKhaoSatListQueryHandler : IQueryHandler<GetKhaoSatListQuery, PagedResult<KhaoSatResponse>>
{
    private readonly IKhaoSatQueryRepository _queryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetKhaoSatListQueryHandler(IKhaoSatQueryRepository queryRepository, ICurrentUserService currentUserService)
    {
        _queryRepository = queryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<KhaoSatResponse>>> Handle(GetKhaoSatListQuery query, CancellationToken cancellationToken)
    {
        var spec = new GetKhaoSatListSpecification(
            query.TrangThaiId,
            query.LoaiKhaoSatId,
            query.Keyword,
            query.NgayTaoTu,
            query.NgayTaoDen,
            query.SortCol,
            query.IsAsc,
            query.PageNumber,
            query.PageSize,
            _currentUserService.UserId);

        var result = await _queryRepository.GetAllAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
