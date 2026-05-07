using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class HoaDonDoiTacCommandRepository : IHoaDonDoiTacCommandRepository
{
    private readonly AppDbContext _dbContext;

    public HoaDonDoiTacCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(HoaDonDoiTac hoaDon, CancellationToken cancellationToken = default)
    {
        await _dbContext.HoaDonDoiTacs.AddAsync(hoaDon, cancellationToken);
    }

    public async Task<HoaDonDoiTac?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HoaDonDoiTacs
            .Include(x => x.FileHoaDon)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsByKyAsync(int hopDongId, int thang, int nam, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HoaDonDoiTacs
            .AnyAsync(h => h.HopDongDoiTacId == hopDongId && h.Thang == thang && h.Nam == nam && !h.IsDeleted, cancellationToken);
    }

    public void Update(HoaDonDoiTac hoaDon)
    {
        _dbContext.HoaDonDoiTacs.Update(hoaDon);
    }

    public void Remove(HoaDonDoiTac hoaDon)
    {
        // Sử dụng Soft Delete
        hoaDon.MarkAsDeleted(DateTimeOffset.Now);
        _dbContext.HoaDonDoiTacs.Update(hoaDon);
    }
}
