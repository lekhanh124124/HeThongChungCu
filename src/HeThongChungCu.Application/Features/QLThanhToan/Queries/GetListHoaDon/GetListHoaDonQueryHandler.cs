using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;

public class GetListHoaDonQueryHandler : IQueryHandler<GetListHoaDonQuery, PagedResult<HoaDonResponse>>
{
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;

    public GetListHoaDonQueryHandler(IHoaDonQueryRepository hoaDonQueryRepository)
    {
        _hoaDonQueryRepository = hoaDonQueryRepository;
    }

    public async Task<Result<PagedResult<HoaDonResponse>>> Handle(GetListHoaDonQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetListHoaDonSpecification(
            request.CanHoId,
            request.DotThanhToanId,
            request.TrangThaiHoaDonId,
            request.Thang,
            request.Nam,
            request.Keyword,
            request.PageNumber,
            request.PageSize,
            request.SortCol,
            request.IsAsc);

        var result = await _hoaDonQueryRepository.GetListAsync(spec, cancellationToken);

        return result;
    }
}
