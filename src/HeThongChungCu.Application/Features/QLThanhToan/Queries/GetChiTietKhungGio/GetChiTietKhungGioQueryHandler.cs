using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietKhungGio;

public class GetChiTietKhungGioQueryHandler : IQueryHandler<GetChiTietKhungGioQuery, ChiTietKhungGioResponse>
{
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;

    public GetChiTietKhungGioQueryHandler(IHoaDonQueryRepository hoaDonQueryRepository)
    {
        _hoaDonQueryRepository = hoaDonQueryRepository;
    }

    public async Task<Result<ChiTietKhungGioResponse>> Handle(GetChiTietKhungGioQuery request, CancellationToken cancellationToken)
    {
        var result = await _hoaDonQueryRepository.GetChiTietKhungGioAsync(request.Id, cancellationToken);

        if (result == null)
        {
            return Result.Failure<ChiTietKhungGioResponse>(HoaDonErrors.InvalidPricingType);
        }

        return result;
    }
}
