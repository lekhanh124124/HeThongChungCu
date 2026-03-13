using HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;

public class LayCuDanByCanHoIdQueryHandler : IQueryHandler<LayCuDanByCanHoIdQuery, IReadOnlyList<CuDanResponse>>
{
    private readonly ICanHoEFRepository _canHoEFRepository;
    private readonly IQuanHeCuTruDapperRepository _queryRepository;

    public LayCuDanByCanHoIdQueryHandler(
        ICanHoEFRepository canHoEFRepository,
        IQuanHeCuTruDapperRepository queryRepository)
    {
        _canHoEFRepository = canHoEFRepository;
        _queryRepository = queryRepository;
    }

    public async Task<Result<IReadOnlyList<CuDanResponse>>> Handle(LayCuDanByCanHoIdQuery request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoEFRepository.AnyAsync(request.CanHoId, cancellationToken);
        if (!canHo)
            return Result.Failure<IReadOnlyList<CuDanResponse>>(CanHoErrors.NotFoundById(request.CanHoId));

        var spec = new LayCuDanByCanHoIdSpecification(request.CanHoId);
        var cuDans = await _queryRepository.GetCuDanByCanHoIdAsync(spec, cancellationToken);
        return Result.Success(cuDans);
    }
}
