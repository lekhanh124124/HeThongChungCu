using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

internal sealed class PhuongTienCommandRepository : IPhuongTienCommandRepository
{
    private readonly AppDbContext _context;

    public PhuongTienCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PhuongTien> AddAsync(PhuongTien phuongTien, CancellationToken cancellationToken = default)
    {
        var result = await _context.PhuongTiens.AddAsync(phuongTien, cancellationToken);
        return result.Entity;
    }

    public void Update(PhuongTien phuongTien)
    {
        _context.PhuongTiens.Update(phuongTien);
    }

    public async Task<PhuongTien?> GetPhuongTienByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.PhuongTiens
            .Include(x => x.ThePhuongTiens.Where(x => !x.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<List<PhuongTien>> GetPhuongTiensByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.PhuongTiens
            .Include(x => x.ThePhuongTiens)
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PhuongTien>> GetPhuongTiensByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _context.PhuongTiens
            .Where(x => x.CanHoId == canHoId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PhuongTien>> GetPhuongTiensByTheIdsAsync(IEnumerable<int> theIds, CancellationToken cancellationToken = default)
    {
        return await _context.PhuongTiens
            .Include(x => x.ThePhuongTiens)
            .Where(x => x.ThePhuongTiens.Any(t => theIds.Contains(t.Id)) && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> BienSoExistsAsync(string bienSo, CancellationToken cancellationToken = default)
    {
        return await _context.PhuongTiens
            .AnyAsync(x => x.BienSo == bienSo && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> MaTheExistsAsync(string maThe, CancellationToken cancellationToken = default)
    {
        return await _context.ThePhuongTiens
            .AnyAsync(x => x.MaThe == maThe && !x.IsDeleted, cancellationToken);
    }

    public async Task<int> GetMaxThePhuongTienIdAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ThePhuongTiens
            .IgnoreQueryFilters() // Usually we want the absolute max including deleted if IDs are reused, but here we just want the highest ID.
            .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;
    }

    public void RemoveRange(IEnumerable<PhuongTien> phuongTiens)
    {
        _context.PhuongTiens.RemoveRange(phuongTiens);
    }
}
