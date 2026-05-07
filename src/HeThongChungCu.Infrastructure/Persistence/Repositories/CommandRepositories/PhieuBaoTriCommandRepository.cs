using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class PhieuBaoTriCommandRepository : IPhieuBaoTriCommandRepository
{
    private readonly AppDbContext _dbContext;

    public PhieuBaoTriCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PhieuBaoTri?> GetPhieuBaoTriByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PhieuBaoTris
            .Include(p => p.Checklists)
            .Include(p => p.VatTus)
            .Include(p => p.NhanSuBaoTris)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> MaPhieuExistsAsync(string maPhieu, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PhieuBaoTris
            .AnyAsync(p => p.MaPhieu == maPhieu, cancellationToken);
    }

    public async Task<bool> ExistsForScheduleOnDateAsync(int scheduleId, DateTimeOffset date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PhieuBaoTris
            .AnyAsync(p => p.LichBaoTriId == scheduleId && p.NgayDuKien.Date == date.Date && !p.IsDeleted, cancellationToken);
    }

    public async Task AddPhieuBaoTriAsync(PhieuBaoTri phieuBaoTri, CancellationToken cancellationToken = default)
    {
        await _dbContext.PhieuBaoTris.AddAsync(phieuBaoTri, cancellationToken);
    }

    public void UpdatePhieuBaoTri(PhieuBaoTri phieuBaoTri)
    {
        _dbContext.PhieuBaoTris.Update(phieuBaoTri);
    }
}
