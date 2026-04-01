using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

public class GetYeuCauCuTruByIdQueryHandler : IQueryHandler<GetYeuCauCuTruByIdQuery, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruQueryRepository _QueryRepository;

    public GetYeuCauCuTruByIdQueryHandler(IYeuCauCuTruQueryRepository QueryRepository)
    {
        _QueryRepository = QueryRepository;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(GetYeuCauCuTruByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _QueryRepository.GetByIdAsync(request.RequestId, cancellationToken);
        
        if (response == null)
            return Result.Failure<YeuCauCuTruResponse>(YeuCauCuTruErrors.NotFound);

        return Result.Success(response);
    }
}
