using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Queries.GetToaNhaById;

public class GetToaNhaByIdQueryHandler : IQueryHandler<GetToaNhaByIdQuery, ToaNhaDetailResponse>
{
    private readonly IToaNhaDapperRepository _queryRepository;

    public GetToaNhaByIdQueryHandler(IToaNhaDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<ToaNhaDetailResponse>> Handle(GetToaNhaByIdQuery request, CancellationToken cancellationToken)
    {
        var toaNha = await _queryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (toaNha is null)
            return Result.Failure<ToaNhaDetailResponse>(ToaNhaErrors.NotFoundById(request.Id));

        return Result.Success(toaNha);
    }
}

