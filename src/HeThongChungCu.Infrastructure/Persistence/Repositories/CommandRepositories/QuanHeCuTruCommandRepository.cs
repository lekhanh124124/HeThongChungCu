using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class QuanHeCuTruCommandRepository : IQuanHeCuTruCommandRepository
{
    private readonly AppDbContext _dbContext;

    public QuanHeCuTruCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuanHeCuTru?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<QuanHeCuTru>()
            .FirstOrDefaultAsync(q =>
                q.Id == id &&
                !q.IsDeleted,
                cancellationToken);
    }

    public async Task<QuanHeCuTru?> GetCuTruByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<QuanHeCuTru>()
            .FirstOrDefaultAsync(q =>
                q.CanHoId == canHoId &&
                !q.IsDeleted &&
                q.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru,
                cancellationToken);
    }

    public async Task AddAsync(QuanHeCuTru quanHeCuTru, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<QuanHeCuTru>().AddAsync(quanHeCuTru, cancellationToken);
    }

    public void Update(QuanHeCuTru quanHeCuTru)
    {
        _dbContext.Set<QuanHeCuTru>().Update(quanHeCuTru);
    }

    public void Remove(QuanHeCuTru quanHeCuTru)
    {
        _dbContext.Set<QuanHeCuTru>().Remove(quanHeCuTru);
    }

    public async Task<IEnumerable<QuanHeCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<QuanHeCuTru>()
            .Where(q =>
                q.CanHoId == canHoId &&
                !q.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuanHeCuTru?> GetByUserAndCanHoAsync(int nguoiDungId, int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<QuanHeCuTru>()
            .FirstOrDefaultAsync(q =>
                q.NguoiDungId == nguoiDungId &&
                q.CanHoId == canHoId &&
                !q.IsDeleted &&
                q.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru,
                cancellationToken);
    }
}
