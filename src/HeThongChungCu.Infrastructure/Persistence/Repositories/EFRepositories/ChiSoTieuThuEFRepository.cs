using Microsoft.EntityFrameworkCore;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Infrastructure.Persistence;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

public class ChiSoTieuThuEFRepository : IChiSoTieuThuEFRepository
{
    private readonly AppDbContext _context;

    public ChiSoTieuThuEFRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ChiSoTieuThu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ChiSoTieuThu>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<ChiSoTieuThu?> GetByThangNamAsync(int canHoId, int dichVuId, int thang, int nam, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ChiSoTieuThu>()
            .FirstOrDefaultAsync(x => x.CanHoId == canHoId && x.DichVuId == dichVuId && x.Thang == thang && x.Nam == nam, cancellationToken);
    }

    public async Task AddAsync(ChiSoTieuThu chiSoTieuThu, CancellationToken cancellationToken = default)
    {
        await _context.Set<ChiSoTieuThu>().AddAsync(chiSoTieuThu, cancellationToken);
    }

    public void Update(ChiSoTieuThu chiSoTieuThu)
    {
        _context.Set<ChiSoTieuThu>().Update(chiSoTieuThu);
    }

    public void Remove(ChiSoTieuThu chiSoTieuThu)
    {
        _context.Set<ChiSoTieuThu>().Remove(chiSoTieuThu);
    }
}
