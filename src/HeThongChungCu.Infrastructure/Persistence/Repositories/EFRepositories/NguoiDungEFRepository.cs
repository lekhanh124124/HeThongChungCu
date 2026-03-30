using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

public class NguoiDungEFRepository : INguoiDungEFRepository
{
    private readonly AppDbContext _dbContext;

    public NguoiDungEFRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NguoiDung?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NguoiDung
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<NguoiDung?> GetByIdWithDocumentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NguoiDung
            .Include(x => x.TaiLieu)
                .ThenInclude(d => d.Files)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<NguoiDung?> GetByCCCDAsync(string cccd, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NguoiDung
            .Include(x => x.TaiLieu)
                .ThenInclude(d => d.Files)
            .FirstOrDefaultAsync(x => x.CCCD == cccd, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<NguoiDung, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NguoiDung.AnyAsync(expression, cancellationToken);
    }

    public async Task AddAsync(NguoiDung nguoiDung, CancellationToken cancellationToken = default)
    {
        await _dbContext.NguoiDung.AddAsync(nguoiDung, cancellationToken);
    }

    public void Update(NguoiDung nguoiDung)
    {
        _dbContext.NguoiDung.Update(nguoiDung);
    }

    public void Delete(NguoiDung nguoiDung)
    {
        _dbContext.NguoiDung.Remove(nguoiDung);
    }
}
