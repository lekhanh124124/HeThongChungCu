using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSCuDanTrongChungCu;

public class LayDSCuDanTrongChungCuQueryHandler : IQueryHandler<LayDSCuDanTrongChungCuQuery, PagedResult<CuDanResponse>>
{
    private readonly ICanHoCommandRepository _canHoCommandRepository;
    private readonly IQuanHeCuTruQueryRepository _queryRepository;

    public LayDSCuDanTrongChungCuQueryHandler(
        ICanHoCommandRepository canHoCommandRepository,
        IQuanHeCuTruQueryRepository queryRepository)
    {
        _canHoCommandRepository = canHoCommandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<CuDanResponse>>> Handle(LayDSCuDanTrongChungCuQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayDSCuDanTrongChungCuQuerySpecification(
            request.ToaNhaId, 
            request.TangId, 
            request.CanHoId,
            request.Keyword,
            request.MaToaNha,
            request.MaTang,
            request.MaCanHo,
            request.LoaiQuanHeCuTruId,
            request.TrangThaiCuTruId,
            request.NgayBatDauFrom,
            request.NgayBatDauTo,
            request.NgayKetThucFrom,
            request.NgayKetThucTo,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var cuDans = await _queryRepository.LayDSCuDanTrongChungCu(spec, cancellationToken);
        return Result.Success(cuDans);
    }
}
