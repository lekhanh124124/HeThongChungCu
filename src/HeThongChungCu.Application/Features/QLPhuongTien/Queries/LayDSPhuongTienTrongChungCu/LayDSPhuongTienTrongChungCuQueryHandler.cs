using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSPhuongTienTrongChungCu;

public class LayDSPhuongTienTrongChungCuQueryHandler : IQueryHandler<LayDSPhuongTienTrongChungCuQuery, PagedResult<PhuongTienResponse>>
{
    private readonly IPhuongTienQueryRepository _queryRepository;

    public LayDSPhuongTienTrongChungCuQueryHandler(IPhuongTienQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<PhuongTienResponse>>> Handle(LayDSPhuongTienTrongChungCuQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayDSPhuongTienTrongChungCuSpecification(
            request.ToaNhaId,
            request.TangId,
            request.CanHoId,
            request.Keyword,
            request.MaToaNha,
            request.MaTang,
            request.MaCanHo,
            request.LoaiPhuongTienId,
            request.MauXe,
            request.TrangThaiPhuongTienId,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize
        );

        var result = await _queryRepository.LayDSPhuongTienTrongChungCu(spec, cancellationToken);
        return result;
    }
}
