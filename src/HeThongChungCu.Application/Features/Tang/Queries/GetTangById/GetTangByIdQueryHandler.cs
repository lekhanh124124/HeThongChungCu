using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Tang.Queries.GetTangById;

public class GetTangByIdQueryHandler : IQueryHandler<GetTangByIdQuery, TangResponse>
{
    private readonly ITangDapperRepository _queryRepository;

    public GetTangByIdQueryHandler(ITangDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<TangResponse>> Handle(GetTangByIdQuery request, CancellationToken cancellationToken)
    {
        var tang = await _queryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (tang is null)
            return Result.Failure<TangResponse>(TangErrors.NotFound);

        return Result.Success(tang);
    }
}
