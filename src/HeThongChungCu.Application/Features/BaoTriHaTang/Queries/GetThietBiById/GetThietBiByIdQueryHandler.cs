using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiById;

public class GetThietBiByIdQueryHandler : IQueryHandler<GetThietBiByIdQuery, ThietBiDetailResponse>
{
    private readonly IThietBiQueryRepository _thietBiQueryRepository;

    public GetThietBiByIdQueryHandler(IThietBiQueryRepository thietBiQueryRepository)
    {
        _thietBiQueryRepository = thietBiQueryRepository;
    }

    public async Task<Result<ThietBiDetailResponse>> Handle(GetThietBiByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetThietBiByIdSpecification(request.Id);
        var result = await _thietBiQueryRepository.GetByIdAsync(spec, cancellationToken);
        if (result == null)
            return BaoTriHaTangErrors.ThietBiNotFoundById(request.Id);

        return Result.Success(result);
    }
}
