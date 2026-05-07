using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class GiaoDichThanhToanCommandRepository : IGiaoDichThanhToanCommandRepository
{
    private readonly AppDbContext _dbContext;

    public GiaoDichThanhToanCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(GiaoDichThanhToan giaoDich, CancellationToken cancellationToken = default)
    {
        await _dbContext.GiaoDichThanhToans.AddAsync(giaoDich, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<GiaoDichThanhToan> giaoDichs, CancellationToken cancellationToken = default)
    {
        await _dbContext.GiaoDichThanhToans.AddRangeAsync(giaoDichs, cancellationToken);
    }

    public async Task<HashSet<int>> GetAllocatedChiTietHoaDonIdsAsync(IEnumerable<int> chiTietHoaDonIds, CancellationToken cancellationToken = default)
    {
        var list = await _dbContext.GiaoDichThanhToans
            .Where(x => chiTietHoaDonIds.Contains(x.ChiTietHoaDonId))
            .Select(x => x.ChiTietHoaDonId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return list.ToHashSet();
    }

    public async Task<decimal> GetPaidAmountByHoaDonIdAsync(int hoaDonId, CancellationToken cancellationToken = default)
    {
        var sum = await _dbContext.GiaoDichThanhToans
            .Join(
                _dbContext.ChiTietHoaDons,
                gd => gd.ChiTietHoaDonId,
                ct => ct.Id,
                (gd, ct) => new { gd.SoTien, ct.HoaDonId })
            .Where(x => x.HoaDonId == hoaDonId)
            .SumAsync(x => (decimal?)x.SoTien, cancellationToken);

        return sum ?? 0m;
    }
}
