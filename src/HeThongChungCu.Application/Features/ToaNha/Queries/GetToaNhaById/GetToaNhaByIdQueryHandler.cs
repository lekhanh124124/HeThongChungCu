using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetToaNhaById;

public class GetToaNhaByIdQueryHandler : IQueryHandler<GetToaNhaByIdQuery, ToaNhaResponse>
{
    private readonly IToaNhaQueryRepository _queryRepository;

    public GetToaNhaByIdQueryHandler(IToaNhaQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<ToaNhaResponse>> Handle(GetToaNhaByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetToaNhaByIdSpecification(request.Id);
        var toaNha = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        if (toaNha is null)
            return Result.Failure<ToaNhaResponse>(ToaNhaErrors.NotFoundById(request.Id));

        return Result.Success(toaNha);
    }
}

