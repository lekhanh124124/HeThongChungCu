
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;

public class GetYeuCauSuaChuaByIdQueryHandler : IQueryHandler<GetYeuCauSuaChuaByIdQuery, YeuCauSuaChuaDetailResponse>
{
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;

    public GetYeuCauSuaChuaByIdQueryHandler(IYeuCauSuaChuaQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<YeuCauSuaChuaDetailResponse>> Handle(GetYeuCauSuaChuaByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetYeuCauSuaChuaByIdSpecification(request.Id);
        var result = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        return result is not null
            ? Result.Success(result)
            : Result.Failure<YeuCauSuaChuaDetailResponse>(YeuCauSuaChuaErrors.NotFound);
    }
}
