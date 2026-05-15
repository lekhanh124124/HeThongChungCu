using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class QuyThuChiCommandRepository : IQuyThuChiCommandRepository
{
    private readonly AppDbContext _dbContext;

    public QuyThuChiCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(QuyThuChi quyThuChi, CancellationToken cancellationToken = default)
    {
        await _dbContext.QuyThuChis.AddAsync(quyThuChi, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<QuyThuChi> quyThuChis, CancellationToken cancellationToken = default)
    {
        await _dbContext.QuyThuChis.AddRangeAsync(quyThuChis, cancellationToken);
    }

    public async Task<QuyThuChi?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.QuyThuChis.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Update(QuyThuChi quyThuChi)
    {
        _dbContext.QuyThuChis.Update(quyThuChi);
    }

    public void Delete(QuyThuChi quyThuChi)
    {
        _dbContext.QuyThuChis.Remove(quyThuChi);
    }
}
