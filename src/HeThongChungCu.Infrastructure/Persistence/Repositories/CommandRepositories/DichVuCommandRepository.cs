using Microsoft.EntityFrameworkCore;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class DichVuCommandRepository : IDichVuCommandRepository
{
    private readonly AppDbContext _context;

    public DichVuCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DichVu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<DichVu>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<DichVu>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<DichVu>().ToListAsync(cancellationToken);
    }

    public async Task<bool> MaDichVuExistsAsync(string maDichVu, CancellationToken cancellationToken = default)
    {
        return await _context.DichVus.AnyAsync(x => x.MaDichVu == maDichVu && !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(DichVu dichVu, CancellationToken cancellationToken = default)
    {
        await _context.Set<DichVu>().AddAsync(dichVu, cancellationToken);
    }

    public void Update(DichVu dichVu)
    {
        _context.Set<DichVu>().Update(dichVu);
    }

    public void Remove(DichVu dichVu)
    {
        _context.Set<DichVu>().Remove(dichVu);
    }
}
