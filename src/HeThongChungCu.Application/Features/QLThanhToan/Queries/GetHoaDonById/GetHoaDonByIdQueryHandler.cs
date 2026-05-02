using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;

public class GetHoaDonByIdQueryHandler : IQueryHandler<GetHoaDonByIdQuery, HoaDonDetailResponse>
{
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;

    public GetHoaDonByIdQueryHandler(IHoaDonQueryRepository hoaDonQueryRepository)
    {
        _hoaDonQueryRepository = hoaDonQueryRepository;
    }

    public async Task<Result<HoaDonDetailResponse>> Handle(GetHoaDonByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetHoaDonByIdSpecification(request.Id);

        var result = await _hoaDonQueryRepository.GetByIdAsync(spec, cancellationToken);

        if (result == null)
        {
            return Result.Failure<HoaDonDetailResponse>(HoaDonErrors.NotFound);
        }

        return result;
    }
}
