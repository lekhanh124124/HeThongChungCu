using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IPhieuBaoTriCommandRepository
{
    Task<PhieuBaoTri?> GetPhieuBaoTriByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaPhieuExistsAsync(string maPhieu, CancellationToken cancellationToken = default);
    Task<bool> ExistsForScheduleOnDateAsync(int scheduleId, DateTimeOffset date, CancellationToken cancellationToken = default);
    Task AddPhieuBaoTriAsync(PhieuBaoTri phieuBaoTri, CancellationToken cancellationToken = default);
    void UpdatePhieuBaoTri(PhieuBaoTri phieuBaoTri);
}
