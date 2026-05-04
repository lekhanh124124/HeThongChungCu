using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietDienTich;

public class GetChiTietDienTichQueryHandler : IQueryHandler<GetChiTietDienTichQuery, ChiTietDienTichResponse>
{
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;

    public GetChiTietDienTichQueryHandler(IHoaDonQueryRepository hoaDonQueryRepository)
    {
        _hoaDonQueryRepository = hoaDonQueryRepository;
    }

    public async Task<Result<ChiTietDienTichResponse>> Handle(GetChiTietDienTichQuery request, CancellationToken cancellationToken)
    {
        var result = await _hoaDonQueryRepository.GetChiTietDienTichAsync(request.Id, cancellationToken);

        if (result == null)
        {
            return Result.Failure<ChiTietDienTichResponse>(HoaDonErrors.InvalidPricingType);
        }

        return result;
    }
}
