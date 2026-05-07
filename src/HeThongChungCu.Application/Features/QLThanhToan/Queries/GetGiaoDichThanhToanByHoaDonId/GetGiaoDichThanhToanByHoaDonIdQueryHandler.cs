using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetGiaoDichThanhToanByHoaDonId;

public class GetGiaoDichThanhToanByHoaDonIdQueryHandler
    : IQueryHandler<GetGiaoDichThanhToanByHoaDonIdQuery, List<GiaoDichThanhToanResponse>>
{
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IGiaoDichThanhToanQueryRepository _giaoDichRepository;

    public GetGiaoDichThanhToanByHoaDonIdQueryHandler(
        IHoaDonCommandRepository hoaDonRepository,
        IGiaoDichThanhToanQueryRepository giaoDichRepository)
    {
        _hoaDonRepository = hoaDonRepository;
        _giaoDichRepository = giaoDichRepository;
    }

    public async Task<Result<List<GiaoDichThanhToanResponse>>> Handle(
        GetGiaoDichThanhToanByHoaDonIdQuery request,
        CancellationToken cancellationToken)
    {
        var hoaDon = await _hoaDonRepository.GetByIdAsync(request.HoaDonId, cancellationToken);
        if (hoaDon == null)
            return HoaDonErrors.NotFound;

        var list = await _giaoDichRepository.GetByHoaDonIdAsync(request.HoaDonId, cancellationToken);
        return Result.Success(list);
    }
}
