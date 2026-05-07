using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class PhienThanhToanCommandRepository : IPhienThanhToanCommandRepository
{
    private readonly AppDbContext _dbContext;

    public PhienThanhToanCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PhienThanhToan phien, CancellationToken cancellationToken = default)
    {
        await _dbContext.PhienThanhToans.AddAsync(phien, cancellationToken);
    }

    public void Update(PhienThanhToan phien)
    {
        _dbContext.PhienThanhToans.Update(phien);
    }

    public async Task<PhienThanhToan?> GetByMaThanhToanAsync(string maThanhToan, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PhienThanhToans
            .FirstOrDefaultAsync(x => x.MaThanhToan == maThanhToan, cancellationToken);
    }
}
