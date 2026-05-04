using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietLuyTien;

public class GetChiTietLuyTienQueryHandler : IQueryHandler<GetChiTietLuyTienQuery, ChiTietLuyTienResponse>
{
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;

    public GetChiTietLuyTienQueryHandler(IHoaDonQueryRepository hoaDonQueryRepository)
    {
        _hoaDonQueryRepository = hoaDonQueryRepository;
    }

    public async Task<Result<ChiTietLuyTienResponse>> Handle(GetChiTietLuyTienQuery request, CancellationToken cancellationToken)
    {
        var result = await _hoaDonQueryRepository.GetChiTietLuyTienAsync(request.Id, cancellationToken);

        if (result == null)
        {
            return Result.Failure<ChiTietLuyTienResponse>(HoaDonErrors.InvalidPricingType);
        }

        return result;
    }
}
