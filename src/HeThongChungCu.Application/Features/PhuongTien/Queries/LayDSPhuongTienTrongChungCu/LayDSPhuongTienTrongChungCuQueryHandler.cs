using HeThongChungCu.Application.Features.PhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.PhuongTien.Queries.LayDSPhuongTienTrongChungCu;

public class LayDSPhuongTienTrongChungCuQueryHandler : IQueryHandler<LayDSPhuongTienTrongChungCuQuery, PagedResult<PhuongTienResponse>>
{
    private readonly IPhuongTienDapperRepository _queryRepository;

    public LayDSPhuongTienTrongChungCuQueryHandler(IPhuongTienDapperRepository queryRepository)
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
        return Result.Success(result);
    }
}
