using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

internal sealed class PhuongTienEFRepository : IPhuongTienEFRepository
{
    private readonly AppDbContext _context;

    public PhuongTienEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PhuongTien phuongTien, CancellationToken cancellationToken = default)
    {
        await _context.PhuongTiens.AddAsync(phuongTien, cancellationToken);
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

    public void RemoveRange(IEnumerable<PhuongTien> phuongTiens)
    {
        _context.PhuongTiens.RemoveRange(phuongTiens);
    }
}
