using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.ChungCu.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ChungCu.Queries.GetCanHoById;

public class GetCanHoByIdQueryHandler : IQueryHandler<GetCanHoByIdQuery, CanHoDetailResponse>
{
    private readonly ICanHoDapperRepository _queryRepository;

    public GetCanHoByIdQueryHandler(ICanHoDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<CanHoDetailResponse>> Handle(GetCanHoByIdQuery request, CancellationToken cancellationToken)
    {
        var canHo = await _queryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (canHo is null)
            return Result.Failure<CanHoDetailResponse>(CanHoErrors.NotFoundById(request.Id));

        return Result.Success(canHo);
    }
}
