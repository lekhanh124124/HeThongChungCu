using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;

public class LayCuDanByCanHoIdQueryHandler : IQueryHandler<LayCuDanByCanHoIdQuery, IReadOnlyList<CuDanResponse>>
{
    private readonly ICanHoDapperRepository _canHoRepository;
    private readonly IQuanHeCuTruDapperRepository _queryRepository;

    public LayCuDanByCanHoIdQueryHandler(
        ICanHoDapperRepository canHoRepository,
        IQuanHeCuTruDapperRepository queryRepository)
    {
        _canHoRepository = canHoRepository;
        _queryRepository = queryRepository;
    }

    public async Task<Result<IReadOnlyList<CuDanResponse>>> Handle(LayCuDanByCanHoIdQuery request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        if (canHo is null)
            return Result.Failure<IReadOnlyList<CuDanResponse>>(CanHoErrors.NotFoundById(request.CanHoId));

        var cuDans = await _queryRepository.GetCuDanByCanHoIdAsync(request.CanHoId, cancellationToken);
        return Result.Success(cuDans);
    }
}
