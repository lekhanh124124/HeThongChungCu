using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;

public class GetCanHoByIdQueryHandler : IQueryHandler<GetCanHoByIdQuery, CanHoResponse>
{
    private readonly ICanHoQueryRepository _queryRepository;

    public GetCanHoByIdQueryHandler(ICanHoQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<CanHoResponse>> Handle(GetCanHoByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetCanHoByIdSpecification(request.Id);
        var canHo = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        if (canHo is null)
            return Result.Failure<CanHoResponse>(CanHoErrors.NotFoundById(request.Id));

        return Result.Success(canHo);
    }
}
