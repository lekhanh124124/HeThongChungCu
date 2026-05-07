using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListHoaDonDoiTac;

public class GetListHoaDonDoiTacQueryHandler : IQueryHandler<GetListHoaDonDoiTacQuery, PagedResult<HoaDonDoiTacResponse>>
{
    private readonly IHoaDonDoiTacQueryRepository _queryRepository;

    public GetListHoaDonDoiTacQueryHandler(IHoaDonDoiTacQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<HoaDonDoiTacResponse>>> Handle(
        GetListHoaDonDoiTacQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new GetListHoaDonDoiTacSpecification(
            request.DoiTacId,
            request.HopDongDoiTacId,
            request.Thang,
            request.Nam,
            request.TrangThaiThanhToanId,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetListAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
