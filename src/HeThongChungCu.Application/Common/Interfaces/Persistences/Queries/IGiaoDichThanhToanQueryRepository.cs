using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IGiaoDichThanhToanQueryRepository
{
    Task<List<GiaoDichThanhToanResponse>> GetByHoaDonIdAsync(int hoaDonId, CancellationToken cancellationToken = default);
}
