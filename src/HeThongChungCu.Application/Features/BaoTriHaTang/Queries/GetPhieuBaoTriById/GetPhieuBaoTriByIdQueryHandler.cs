using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriById;

public class GetPhieuBaoTriByIdQueryHandler : IQueryHandler<GetPhieuBaoTriByIdQuery, PhieuBaoTriDetailResponse>
{
    private readonly IPhieuBaoTriQueryRepository _queryRepository;

    public GetPhieuBaoTriByIdQueryHandler(IPhieuBaoTriQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PhieuBaoTriDetailResponse>> Handle(GetPhieuBaoTriByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetPhieuBaoTriByIdSpecification(request.Id);
        var result = await _queryRepository.GetByIdAsync(spec, cancellationToken);
        if (result == null)
            return BaoTriHaTangErrors.PhieuBaoTriNotFoundById(request.Id);

        return Result.Success(result);
    }
}
