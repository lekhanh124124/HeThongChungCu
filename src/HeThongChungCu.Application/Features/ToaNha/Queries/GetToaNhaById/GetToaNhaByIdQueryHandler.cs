using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetToaNhaById;

public class GetToaNhaByIdQueryHandler : IQueryHandler<GetToaNhaByIdQuery, ToaNhaResponse>
{
    private readonly IToaNhaDapperRepository _queryRepository;

    public GetToaNhaByIdQueryHandler(IToaNhaDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<ToaNhaResponse>> Handle(GetToaNhaByIdQuery request, CancellationToken cancellationToken)
    {
        var toaNha = await _queryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (toaNha is null)
            return Result.Failure<ToaNhaResponse>(ToaNhaErrors.NotFoundById(request.Id));

        return Result.Success(toaNha);
    }
}

