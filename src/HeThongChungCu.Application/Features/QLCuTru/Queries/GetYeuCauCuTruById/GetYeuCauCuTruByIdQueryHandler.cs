using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

public class GetYeuCauCuTruByIdQueryHandler : IQueryHandler<GetYeuCauCuTruByIdQuery, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruQueryRepository _yeuCauQueryRepository;

    public GetYeuCauCuTruByIdQueryHandler(IYeuCauCuTruQueryRepository yeuCauQueryRepository)
    {
        _yeuCauQueryRepository = yeuCauQueryRepository;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(GetYeuCauCuTruByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetYeuCauCuTruByIdSpecification(request.RequestId);
        var response = await _yeuCauQueryRepository.GetByIdAsync(spec, cancellationToken);
        
        if (response == null)
            return Result.Failure<YeuCauCuTruResponse>(YeuCauCuTruErrors.NotFound);

        return Result.Success(response);
    }
}
