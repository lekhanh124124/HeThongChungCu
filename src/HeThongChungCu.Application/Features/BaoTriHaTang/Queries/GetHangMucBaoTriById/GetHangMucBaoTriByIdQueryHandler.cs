using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;

public class GetHangMucBaoTriByIdQueryHandler : IQueryHandler<GetHangMucBaoTriByIdQuery, HangMucBaoTriDetailResponse>
{
    private readonly IHangMucBaoTriQueryRepository _queryRepository;

    public GetHangMucBaoTriByIdQueryHandler(IHangMucBaoTriQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<HangMucBaoTriDetailResponse>> Handle(GetHangMucBaoTriByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetHangMucBaoTriByIdSpecification(request.Id);
        var result = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        return result is not null 
            ? Result.Success(result)
            : Result.Failure<HangMucBaoTriDetailResponse>(BaoTriHaTangErrors.HangMucNotFoundById(request.Id));
    }
}
