using Microsoft.EntityFrameworkCore;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Infrastructure.Persistence;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class BangGiaCommandRepository : IBangGiaCommandRepository
{
    private readonly AppDbContext _context;

    public BangGiaCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BangGia?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<BangGia>()
            .Include(x => x.BangGiaLuyTiens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<BangGia>> GetByDichVuIdAsync(int dichVuId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<BangGia>()
            .Where(x => x.DichVuId == dichVuId)
            .Include(x => x.BangGiaLuyTiens)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BangGia bangGia, CancellationToken cancellationToken = default)
    {
        await _context.Set<BangGia>().AddAsync(bangGia, cancellationToken);
    }

    public void Update(BangGia bangGia)
    {
        _context.Set<BangGia>().Update(bangGia);
    }

    public void Remove(BangGia bangGia)
    {
        _context.Set<BangGia>().Remove(bangGia);
    }
}
