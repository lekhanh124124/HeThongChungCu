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
        return await _dbContext.QuanHeCuTrus
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<QuanHeCuTru?> GetCuTruByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.QuanHeCuTrus
            .FirstOrDefaultAsync(q =>
                q.CanHoId == canHoId &&
                q.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru,
                cancellationToken);
    }

    public async Task AddAsync(QuanHeCuTru quanHeCuTru, CancellationToken cancellationToken = default)
    {
        await _dbContext.QuanHeCuTrus.AddAsync(quanHeCuTru, cancellationToken);
    }

    public void Update(QuanHeCuTru quanHeCuTru)
    {
        _dbContext.QuanHeCuTrus.Update(quanHeCuTru);
    }

    public void Remove(QuanHeCuTru quanHeCuTru)
    {
        _dbContext.QuanHeCuTrus.Remove(quanHeCuTru);
    }

    public async Task<IEnumerable<QuanHeCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.QuanHeCuTrus
            .Where(q => q.CanHoId == canHoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuanHeCuTru?> GetByUserAndCanHoAsync(int nguoiDungId, int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.QuanHeCuTrus
            .FirstOrDefaultAsync(q =>
                q.NguoiDungId == nguoiDungId &&
                q.CanHoId == canHoId &&
                q.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru,
                cancellationToken);
    }
}
