using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietCoDinh;

public class GetChiTietCoDinhQueryHandler : IQueryHandler<GetChiTietCoDinhQuery, ChiTietCoDinhResponse>
{
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;

    public GetChiTietCoDinhQueryHandler(IHoaDonQueryRepository hoaDonQueryRepository)
    {
        _hoaDonQueryRepository = hoaDonQueryRepository;
    }

    public async Task<Result<ChiTietCoDinhResponse>> Handle(GetChiTietCoDinhQuery request, CancellationToken cancellationToken)
    {
        var result = await _hoaDonQueryRepository.GetChiTietCoDinhAsync(request.Id, cancellationToken);

        if (result == null)
        {
            return Result.Failure<ChiTietCoDinhResponse>(HoaDonErrors.InvalidPricingType);
        }

        return result;
    }
}
