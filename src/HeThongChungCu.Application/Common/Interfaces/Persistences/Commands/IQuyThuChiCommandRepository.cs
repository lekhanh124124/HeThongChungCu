using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IQuyThuChiCommandRepository
{
    Task AddAsync(QuyThuChi quyThuChi, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<QuyThuChi> quyThuChis, CancellationToken cancellationToken = default);
    Task<QuyThuChi?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    void Update(QuyThuChi quyThuChi);
    void Delete(QuyThuChi quyThuChi);
}
