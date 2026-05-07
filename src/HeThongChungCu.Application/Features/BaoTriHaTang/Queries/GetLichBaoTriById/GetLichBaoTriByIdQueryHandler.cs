using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriById;

public class GetLichBaoTriByIdQueryHandler : IQueryHandler<GetLichBaoTriByIdQuery, LichBaoTriDetailResponse>
{
    private readonly ILichBaoTriQueryRepository _queryRepository;

    public GetLichBaoTriByIdQueryHandler(ILichBaoTriQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<LichBaoTriDetailResponse>> Handle(GetLichBaoTriByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetLichBaoTriByIdSpecification(request.Id);
        var result = await _queryRepository.GetByIdAsync(spec, cancellationToken);
        if (result == null)
            return BaoTriHaTangErrors.LichBaoTriNotFoundById(request.Id);

        return Result.Success(result);
    }
}
