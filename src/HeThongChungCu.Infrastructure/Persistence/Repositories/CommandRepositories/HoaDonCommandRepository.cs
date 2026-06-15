using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class HoaDonCommandRepository : IHoaDonCommandRepository
{
    private readonly AppDbContext _dbContext;

    public HoaDonCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(HoaDon hoaDon, CancellationToken cancellationToken = default)
    {
        await _dbContext.HoaDons.AddAsync(hoaDon, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<HoaDon> hoaDons, CancellationToken cancellationToken = default)
    {
        await _dbContext.HoaDons.AddRangeAsync(hoaDons, cancellationToken);
    }

    public async Task<HoaDon?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HoaDons
            .Include(x => x.ChiTietHoaDons)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<HoaDon>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HoaDons
            .Include(x => x.ChiTietHoaDons)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<HoaDon>> GetByDotThanhToanAsync(int dotId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HoaDons
            .Include(x => x.ChiTietHoaDons)
            .Where(x => x.DotThanhToanId == dotId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByKyThanhToanAsync(int canHoId, KyThanhToan ky, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HoaDons
            .AnyAsync(x => x.CanHoId == canHoId && 
                           x.KyThanhToan.Thang == ky.Thang && 
                           x.KyThanhToan.Nam == ky.Nam, 
                      cancellationToken);
    }

    public async Task<HashSet<int>> GetExistingCanHoIdsByKyAsync(KyThanhToan ky, CancellationToken cancellationToken = default)
    {
        var ids = await _dbContext.HoaDons
            .Where(x => x.KyThanhToan.Thang == ky.Thang && x.KyThanhToan.Nam == ky.Nam)
            .Select(x => x.CanHoId)
            .ToListAsync(cancellationToken);
            
        return ids.ToHashSet();
    }

    public async Task<bool> AnyByDotThanhToanAsync(int dotId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HoaDons.AnyAsync(x => x.DotThanhToanId == dotId, cancellationToken);
    }

    public async Task<bool> HasUnpaidInvoicesAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        var unpaidStatuses = new[]
        {
            TrangThaiHoaDon.ChuaThanhToan,
            TrangThaiHoaDon.ThanhToanMotPhan,
            TrangThaiHoaDon.QuaHan,
            TrangThaiHoaDon.QuaHanNhe,
            TrangThaiHoaDon.QuaHanNang
        };

        return await _dbContext.HoaDons
            .AnyAsync(x => x.CanHoId == canHoId && unpaidStatuses.Contains(x.TrangThaiHoaDonId), cancellationToken);
    }

    public async Task<bool> AnyInvoicesByCanHoAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HoaDons.AnyAsync(x => x.CanHoId == canHoId, cancellationToken);
    }

    public async Task<ILookup<int, HoaDon>> GetOverdueByCanHoIdsAsync(
        IEnumerable<int> canHoIds,
        DateTimeOffset dotStartDate,
        CancellationToken cancellationToken = default)
    {
        var today = DateTimeOffset.Now;
        var overdueStatuses = new[]
        {
            TrangThaiHoaDon.ChuaThanhToan,
            TrangThaiHoaDon.ThanhToanMotPhan,
            TrangThaiHoaDon.QuaHan,
            TrangThaiHoaDon.QuaHanNhe,
            TrangThaiHoaDon.QuaHanNang
        };

        var list = await _dbContext.HoaDons
            .Where(x =>
                canHoIds.Contains(x.CanHoId) &&
                overdueStatuses.Contains(x.TrangThaiHoaDonId) &&
                x.NgayHanThanhToan < today &&
                (x.NgayTinhLaiCuoi == null || x.NgayTinhLaiCuoi < dotStartDate))
            .ToListAsync(cancellationToken);

        return list.ToLookup(x => x.CanHoId);
    }

    public async Task<List<HoaDon>> GetPendingPastDueInvoicesAsync(DateTimeOffset referenceDate, CancellationToken cancellationToken = default)
    {
        var pendingStatuses = new[]
        {
            TrangThaiHoaDon.ChuaThanhToan,
            TrangThaiHoaDon.ThanhToanMotPhan
        };

        return await _dbContext.HoaDons
            .Where(x => pendingStatuses.Contains(x.TrangThaiHoaDonId) && x.NgayHanThanhToan < referenceDate)
            .ToListAsync(cancellationToken);
    }

    public void Update(HoaDon hoaDon)
    {
        _dbContext.HoaDons.Update(hoaDon);
    }
}
