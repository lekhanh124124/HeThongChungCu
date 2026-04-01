using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.LayDSDangKyDichVuCuaCanHo;

public sealed record LayDSDangKyDichVuCuaCanHoQuery(int CanHoId) : IQuery<IReadOnlyList<DangKyDichVuResponse>>;

internal sealed class LayDSDangKyDichVuCuaCanHoQueryHandler : IQueryHandler<LayDSDangKyDichVuCuaCanHoQuery, IReadOnlyList<DangKyDichVuResponse>>
{
    private readonly IDangKyDichVuCommandRepository _dangKyDichVuRepository;

    public LayDSDangKyDichVuCuaCanHoQueryHandler(IDangKyDichVuCommandRepository dangKyDichVuRepository)
    {
        _dangKyDichVuRepository = dangKyDichVuRepository;
    }

    public async Task<Result<IReadOnlyList<DangKyDichVuResponse>>> Handle(LayDSDangKyDichVuCuaCanHoQuery request, CancellationToken cancellationToken)
    {
        var registrations = await _dangKyDichVuRepository.GetByCanHoIdAsync(request.CanHoId, cancellationToken);
        
        var response = registrations.Select(r => new DangKyDichVuResponse(
            r.Id,
            r.CanHoId,
            r.DichVuId,
            r.NgayBatDau,
            r.NgayKetThuc,
            r.IsActive)).ToList();

        return response;
    }
}
