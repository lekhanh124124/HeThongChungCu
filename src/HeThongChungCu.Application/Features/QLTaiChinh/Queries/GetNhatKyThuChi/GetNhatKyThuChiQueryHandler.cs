using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using HeThongChungCu.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetNhatKyThuChi;

public class GetNhatKyThuChiQueryHandler : IQueryHandler<GetNhatKyThuChiQuery, PagedResult<QuyThuChiResponse>>
{
    private readonly IQuyThuChiQueryRepository _queryRepository;

    public GetNhatKyThuChiQueryHandler(IQuyThuChiQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<PagedResult<QuyThuChiResponse>>> Handle(GetNhatKyThuChiQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetNhatKyThuChiSpecification(
            request.LoaiGiaoDichId,
            request.DichVuId,
            request.NhomThongKe,
            request.TuNgay,
            request.DenNgay,
            request.Keyword,
            request.SortCol,
            request.IsAsc,
            request.PageNumber,
            request.PageSize);

        var result = await _queryRepository.GetNhatKyThuChiAsync(spec, cancellationToken);
        return Result.Success(result);
    }
}
