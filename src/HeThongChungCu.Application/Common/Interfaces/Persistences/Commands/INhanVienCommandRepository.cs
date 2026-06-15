using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface INhanVienCommandRepository
{
    Task AddAsync(NhanVien nhanVien, CancellationToken cancellationToken = default);
    Task UpdateAsync(NhanVien nhanVien, CancellationToken cancellationToken = default);
    Task DeleteAsync(NhanVien nhanVien, CancellationToken cancellationToken = default);
    Task<NhanVien?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NhanVien>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<bool> HasAssignedRequestsAsync(int nhanVienId, CancellationToken cancellationToken = default);
    Task<bool> MaNhanVienExistsAsync(string maNhanVien, CancellationToken cancellationToken = default);
    Task<NhanVien?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
