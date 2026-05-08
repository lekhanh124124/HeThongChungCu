using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class KhaoSatCommandRepository : IKhaoSatCommandRepository
{
    private readonly AppDbContext _dbContext;

    public KhaoSatCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<KhaoSat?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.KhaoSats
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<KhaoSat?> GetByIdWithQuestionsAndChoicesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.KhaoSats
            .Include(x => x.CauHois)
                .ThenInclude(q => q.LuaChons)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<KhaoSat?> GetByIdWithVotesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.KhaoSats
            .Include(x => x.BieuQuyets)
                .ThenInclude(v => v.ChiTiets)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<KhaoSat?> GetByIdWithAllAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.KhaoSats
            .Include(x => x.CauHois)
                .ThenInclude(q => q.LuaChons)
            .Include(x => x.BieuQuyets)
                .ThenInclude(v => v.ChiTiets)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<KhaoSat>> GetExpiredCampaignsAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return await _dbContext.KhaoSats
            .Where(x => x.TrangThaiId == HeThongChungCu.Domain.Enums.TrangThaiKhaoSat.DangDienRa && x.NgayKetThuc < now)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(KhaoSat khaoSat, CancellationToken cancellationToken = default)
    {
        await _dbContext.KhaoSats.AddAsync(khaoSat, cancellationToken);
    }

    public void Update(KhaoSat khaoSat)
    {
        _dbContext.KhaoSats.Update(khaoSat);
    }

    public void Delete(KhaoSat khaoSat)
    {
        _dbContext.KhaoSats.Remove(khaoSat);
    }
}
