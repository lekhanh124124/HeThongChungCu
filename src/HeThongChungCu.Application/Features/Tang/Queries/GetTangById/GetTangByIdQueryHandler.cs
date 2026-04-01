using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Queries.GetTangById;

public class GetTangByIdQueryHandler : IQueryHandler<GetTangByIdQuery, TangResponse>
{
    private readonly IToaNhaQueryRepository _queryRepository;

    public GetTangByIdQueryHandler(IToaNhaQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<TangResponse>> Handle(GetTangByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetTangByIdSpecification(request.Id);
        var tang = await _queryRepository.GetTangDetailByIdAsync(spec, cancellationToken);

        if (tang is null)
            return Result.Failure<TangResponse>(TangErrors.NotFound);

        return Result.Success(tang);
    }
}
