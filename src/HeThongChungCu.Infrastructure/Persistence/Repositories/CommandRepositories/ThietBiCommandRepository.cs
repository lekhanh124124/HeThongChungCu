using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class ThietBiCommandRepository : IThietBiCommandRepository
{
    private readonly AppDbContext _dbContext;

    public ThietBiCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ThietBi?> GetThietBiByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ThietBis
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<bool> MaThietBiExistsAsync(string maThietBi, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ThietBis
            .AnyAsync(t => t.MaThietBi == maThietBi, cancellationToken);
    }

    public async Task AddThietBiAsync(ThietBi thietBi, CancellationToken cancellationToken = default)
    {
        await _dbContext.ThietBis.AddAsync(thietBi, cancellationToken);
    }

    public void UpdateThietBi(ThietBi thietBi)
    {
        _dbContext.ThietBis.Update(thietBi);
    }

    public async Task<HangMucBaoTri?> GetHangMucByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HangMucBaoTris
            .FirstOrDefaultAsync(hm => hm.Id == id, cancellationToken);
    }

    public async Task<bool> MaHangMucExistsAsync(string maHangMuc, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HangMucBaoTris
            .AnyAsync(hm => hm.MaHangMuc == maHangMuc, cancellationToken);
    }

    public async Task AddHangMucAsync(HangMucBaoTri hangMuc, CancellationToken cancellationToken = default)
    {
        await _dbContext.HangMucBaoTris.AddAsync(hangMuc, cancellationToken);
    }

    public void UpdateHangMuc(HangMucBaoTri hangMuc)
    {
        _dbContext.HangMucBaoTris.Update(hangMuc);
    }

    public async Task<LichBaoTri?> GetLichBaoTriByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LichBaoTris
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<List<LichBaoTri>> GetActiveLichBaoTrisAsync(DateTimeOffset date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LichBaoTris
            .Where(l => l.IsActive && l.NgayBaoTriTiepTheo <= date)
            .ToListAsync(cancellationToken);
    }

    public async Task AddLichBaoTriAsync(LichBaoTri lichBaoTri, CancellationToken cancellationToken = default)
    {
        await _dbContext.LichBaoTris.AddAsync(lichBaoTri, cancellationToken);
    }

    public void UpdateLichBaoTri(LichBaoTri lichBaoTri)
    {
        _dbContext.LichBaoTris.Update(lichBaoTri);
    }
}
