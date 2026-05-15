using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class YeuCauPhanAnhCommandRepository : IYeuCauPhanAnhCommandRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauPhanAnhCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<YeuCauPhanAnh?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauPhanAnhs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<YeuCauPhanAnh?> GetByIdWithRepliesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauPhanAnhs
            .Include(x => x.TraLoiPhanAnhs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<YeuCauPhanAnh?> GetByIdWithFilesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauPhanAnhs
            .Include(x => x.TepYeuCauPhanAnhs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<YeuCauPhanAnh?> GetByIdWithAllAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauPhanAnhs
            .Include(x => x.TraLoiPhanAnhs)
            .Include(x => x.TepYeuCauPhanAnhs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<YeuCauPhanAnh>> GetOverdueNotNotifiedAsync(DateTimeOffset currentTime, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauPhanAnhs
            .Where(x => !x.IsQuaHanNotified
                        && x.HanPhanHoi != null
                        && x.HanPhanHoi < currentTime
                        && (x.TrangThaiPhanAnhId == HeThongChungCu.Domain.Enums.TrangThaiPhanAnh.ChoTiepNhan || 
                            x.TrangThaiPhanAnhId == HeThongChungCu.Domain.Enums.TrangThaiPhanAnh.DangXuLy ||
                            x.TrangThaiPhanAnhId == HeThongChungCu.Domain.Enums.TrangThaiPhanAnh.CuDanPhanHoi))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(YeuCauPhanAnh phanAnh, CancellationToken cancellationToken = default)
    {
        await _dbContext.YeuCauPhanAnhs.AddAsync(phanAnh, cancellationToken);
    }

    public void Update(YeuCauPhanAnh phanAnh)
    {
        _dbContext.YeuCauPhanAnhs.Update(phanAnh);
    }

    public void Delete(YeuCauPhanAnh phanAnh)
    {
        _dbContext.YeuCauPhanAnhs.Remove(phanAnh);
    }
}
