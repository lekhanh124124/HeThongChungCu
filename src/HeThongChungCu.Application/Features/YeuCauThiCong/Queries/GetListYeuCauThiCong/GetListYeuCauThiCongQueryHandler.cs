using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetListYeuCauThiCong;

public class GetListYeuCauThiCongQueryHandler : IQueryHandler<GetListYeuCauThiCongQuery, PagedResult<YeuCauThiCongResponse>>
{
    private readonly IYeuCauThiCongQueryRepository _queryRepository;

    public GetListYeuCauThiCongQueryHandler(IYeuCauThiCongQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<YeuCauThiCongResponse>>> Handle(GetListYeuCauThiCongQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListYeuCauThiCongSpecification(
            request.CanHoId,
            request.TrangThaiId,
            request.TrangThaiThiCongId,
            request.Keyword,
            request.NgayTaoTu,
            request.NgayTaoDen,
            request.BatDauTu,
            request.BatDauDen,
            request.KetThucTu,
            request.KetThucDen,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize,
            request.MaCanHo,
            request.TenNguoiGui);

        return Result.Success(await _queryRepository.GetAllAsync(spec, cancellationToken));
    }
}
