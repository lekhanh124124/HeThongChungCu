using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetListYeuCauSuaChua;

public class GetListYeuCauSuaChuaQueryHandler : IQueryHandler<GetListYeuCauSuaChuaQuery, PagedResult<YeuCauSuaChuaResponse>>
{
    private readonly IYeuCauSuaChuaQueryRepository _queryRepository;

    public GetListYeuCauSuaChuaQueryHandler(IYeuCauSuaChuaQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<YeuCauSuaChuaResponse>>> Handle(GetListYeuCauSuaChuaQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListYeuCauSuaChuaSpecification(
            request.PageNumber,
            request.PageSize,
            request.SortCol,
            request.IsAsc,
            request.CanHoId,
            request.TrangThaiYeuCauId,
            request.TrangThaiSuaChuaId,
            request.LoaiSuCoId,
            request.NgayTaoTu,
            request.NgayTaoDen,
            request.MaCanHo,
            request.TenNguoiGui);

        var result = await _queryRepository.GetAllAsync(spec, cancellationToken);
        return result;
    }
}
