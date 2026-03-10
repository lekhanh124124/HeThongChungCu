using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;

public class GetCanHoByIdQueryHandler : IQueryHandler<GetCanHoByIdQuery, CanHoResponse>
{
    private readonly ICanHoDapperRepository _queryRepository;

    public GetCanHoByIdQueryHandler(ICanHoDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<CanHoResponse>> Handle(GetCanHoByIdQuery request, CancellationToken cancellationToken)
    {
        var canHo = await _queryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (canHo is null)
            return Result.Failure<CanHoResponse>(CanHoErrors.NotFoundById(request.Id));

        return Result.Success(canHo);
    }
}
