using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class ChiSoTieuThuCommandRepository : IChiSoTieuThuCommandRepository
{
    private readonly AppDbContext _dbContext;

    public ChiSoTieuThuCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ChiSoTieuThu chiSo, CancellationToken cancellationToken = default)
    {
        await _dbContext.ChiSoTieuThus.AddAsync(chiSo, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<ChiSoTieuThu> chiSos, CancellationToken cancellationToken = default)
    {
        await _dbContext.ChiSoTieuThus.AddRangeAsync(chiSos, cancellationToken);
    }

    public async Task<ChiSoTieuThu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChiSoTieuThus.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<ChiSoTieuThu>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChiSoTieuThus
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ChiSoTieuThu>> GetLockedUnbilledByPeriodAsync(KyThanhToan ky, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChiSoTieuThus
            .Where(x => x.Thang == ky.Thang && 
                        x.Nam == ky.Nam && 
                        x.TrangThaiChiSoId == TrangThaiChiSo.Confirmed && 
                        x.HoaDonId == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ChiSoTieuThu>> GetLockedUnbilledByCanHoAsync(int canHoId, KyThanhToan ky, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChiSoTieuThus
            .Where(x => x.CanHoId == canHoId && 
                        x.Thang == ky.Thang && 
                        x.Nam == ky.Nam && 
                        x.TrangThaiChiSoId == TrangThaiChiSo.Confirmed && 
                        x.HoaDonId == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<ChiSoTieuThu?> GetLatestByCanHoAndDichVuAsync(int canHoId, int dichVuId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChiSoTieuThus
            .Where(x => x.CanHoId == canHoId && x.DichVuId == dichVuId)
            .OrderByDescending(x => x.Nam)
            .ThenByDescending(x => x.Thang)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<ChiSoTieuThu>> GetByPeriodAsync(int thang, int nam, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChiSoTieuThus
            .Where(x => x.Thang == thang && x.Nam == nam)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ChiSoTieuThu>> GetByMaTraCuusAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChiSoTieuThus
            .Where(x => x.MaTraCuu != null && codes.Contains(x.MaTraCuu))
            .ToListAsync(cancellationToken);
    }

    public void Update(ChiSoTieuThu chiSo)
    {
        _dbContext.ChiSoTieuThus.Update(chiSo);
    }

    public void Remove(ChiSoTieuThu chiSo)
    {
        _dbContext.ChiSoTieuThus.Remove(chiSo);
    }

    public void RemoveRange(IEnumerable<ChiSoTieuThu> chiSos)
    {
        _dbContext.ChiSoTieuThus.RemoveRange(chiSos);
    }
}
