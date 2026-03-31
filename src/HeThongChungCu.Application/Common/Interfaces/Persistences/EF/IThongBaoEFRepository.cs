using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IThongBaoEFRepository
{
    Task<PhanBoThongBao?> GetPhanBoByIdAsync(int phanBoId, int userId, CancellationToken cancellationToken = default);
    Task AddAsync(ThongBao thongBao, CancellationToken cancellationToken = default);
    void UpdatePhanBo(PhanBoThongBao phanBo);
}
