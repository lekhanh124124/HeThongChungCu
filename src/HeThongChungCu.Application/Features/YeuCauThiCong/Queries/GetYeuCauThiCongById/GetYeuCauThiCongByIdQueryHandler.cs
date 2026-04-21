using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;

public class GetYeuCauThiCongByIdQueryHandler : IQueryHandler<GetYeuCauThiCongByIdQuery, YeuCauThiCongDetailResponse?>
{
    private readonly IYeuCauThiCongQueryRepository _queryRepository;

    public GetYeuCauThiCongByIdQueryHandler(IYeuCauThiCongQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<YeuCauThiCongDetailResponse?>> Handle(GetYeuCauThiCongByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetYeuCauThiCongByIdSpecification(request.Id);
        return Result.Success(await _queryRepository.GetByIdAsync(spec, cancellationToken));
    }
}
