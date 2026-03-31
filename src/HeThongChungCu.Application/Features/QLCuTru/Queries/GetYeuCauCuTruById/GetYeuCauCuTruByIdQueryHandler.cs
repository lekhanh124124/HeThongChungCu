using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

public class GetYeuCauCuTruByIdQueryHandler : IQueryHandler<GetYeuCauCuTruByIdQuery, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruDapperRepository _dapperRepository;

    public GetYeuCauCuTruByIdQueryHandler(IYeuCauCuTruDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(GetYeuCauCuTruByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _dapperRepository.GetByIdAsync(request.RequestId, cancellationToken);
        
        if (response == null)
            return Result.Failure<YeuCauCuTruResponse>(YeuCauCuTruErrors.NotFound);

        return Result.Success(response);
    }
}
