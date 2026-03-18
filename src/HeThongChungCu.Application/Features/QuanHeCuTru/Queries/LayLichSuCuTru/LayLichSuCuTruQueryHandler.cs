using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;

public class LayLichSuCuTruQueryHandler : IQueryHandler<LayLichSuCuTruQuery, PagedResult<LichSuCuTruResponse>>
{
    private readonly IQuanHeCuTruDapperRepository _queryRepository;

    public LayLichSuCuTruQueryHandler(IQuanHeCuTruDapperRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<LichSuCuTruResponse>>> Handle(LayLichSuCuTruQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayLichSuCuTruSpecification(
            request.UserId,
            request.LoaiQuanHeCuTruId,
            request.NgayBatDauFrom,
            request.NgayBatDauTo,
            request.NgayKetThucFrom,
            request.NgayKetThucTo,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetLichSuAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
