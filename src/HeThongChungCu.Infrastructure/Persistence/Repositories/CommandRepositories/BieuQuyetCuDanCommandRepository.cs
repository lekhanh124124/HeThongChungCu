using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class BieuQuyetCuDanCommandRepository : IBieuQuyetCuDanCommandRepository
{
    private readonly AppDbContext _dbContext;

    public BieuQuyetCuDanCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BieuQuyetCuDan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BieuQuyetCuDans
            .Include(x => x.ChiTiets)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> HasResidentVotedAsync(int khaoSatId, int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BieuQuyetCuDans
            .AnyAsync(x => x.KhaoSatId == khaoSatId && x.CanHoId == canHoId, cancellationToken);
    }

    public async Task AddAsync(BieuQuyetCuDan bieuQuyet, CancellationToken cancellationToken = default)
    {
        await _dbContext.BieuQuyetCuDans.AddAsync(bieuQuyet, cancellationToken);
    }

    public void Update(BieuQuyetCuDan bieuQuyet)
    {
        _dbContext.BieuQuyetCuDans.Update(bieuQuyet);
    }

    public void Delete(BieuQuyetCuDan bieuQuyet)
    {
        _dbContext.BieuQuyetCuDans.Remove(bieuQuyet);
    }
}
