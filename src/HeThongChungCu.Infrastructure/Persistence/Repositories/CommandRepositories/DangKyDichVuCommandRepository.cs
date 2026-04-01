using Microsoft.EntityFrameworkCore;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class DangKyDichVuCommandRepository : IDangKyDichVuCommandRepository
{
    private readonly AppDbContext _context;

    public DangKyDichVuCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DangKyDichVu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<DangKyDichVu>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<DangKyDichVu?> GetActiveAsync(int canHoId, int dichVuId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<DangKyDichVu>()
            .FirstOrDefaultAsync(x => x.CanHoId == canHoId && x.DichVuId == dichVuId && x.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<DangKyDichVu>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<DangKyDichVu>()
            .Where(x => x.CanHoId == canHoId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(DangKyDichVu dangKyDichVu, CancellationToken cancellationToken = default)
    {
        await _context.Set<DangKyDichVu>().AddAsync(dangKyDichVu, cancellationToken);
    }

    public void Update(DangKyDichVu dangKyDichVu)
    {
        _context.Set<DangKyDichVu>().Update(dangKyDichVu);
    }
}
