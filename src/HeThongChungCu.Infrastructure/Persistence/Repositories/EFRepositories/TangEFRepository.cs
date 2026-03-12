using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

public class TangEFRepository : ITangEFRepository
{
    private readonly AppDbContext _context;

    public TangEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tang?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tangs
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tangs.AnyAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tangs.AnyAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<bool> MaTangExistsAsync(string maTang, CancellationToken cancellationToken = default)
    {
        return await _context.Tangs.AnyAsync(t => t.MaTang == maTang && !t.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Tang>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Tangs
            .Where(t => ids.Contains(t.Id) && !t.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Tang tang, CancellationToken cancellationToken = default)
    {
        await _context.Tangs.AddAsync(tang, cancellationToken);
    }

    public void Update(Tang tang)
    {
        _context.Tangs.Update(tang);
    }

    public void Remove(Tang tang)
    {
        _context.Tangs.Remove(tang);
    }
}
