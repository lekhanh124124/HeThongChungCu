using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

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

        var spec = new LayCuDanByCanHoIdSpecification(request.CanHoId);
        var cuDans = await _queryRepository.GetCuDanByCanHoIdAsync(spec, cancellationToken);
        return Result.Success(cuDans);
    }
}
