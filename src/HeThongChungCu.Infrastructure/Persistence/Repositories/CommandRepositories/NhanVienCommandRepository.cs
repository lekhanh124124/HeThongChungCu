using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class NhanVienCommandRepository : INhanVienCommandRepository
{
    private readonly AppDbContext _dbContext;

    public NhanVienCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(NhanVien nhanVien, CancellationToken cancellationToken = default)
    {
        await _dbContext.NhanViens.AddAsync(nhanVien, cancellationToken);
    }

    public Task UpdateAsync(NhanVien nhanVien, CancellationToken cancellationToken = default)
    {
        _dbContext.NhanViens.Update(nhanVien);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(NhanVien nhanVien, CancellationToken cancellationToken = default)
    {
        _dbContext.NhanViens.Remove(nhanVien);
        return Task.CompletedTask;
    }

    public async Task<NhanVien?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NhanViens
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<NhanVien>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NhanViens
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAssignedRequestsAsync(int nhanVienId, CancellationToken cancellationToken = default)
    {
        var hasYeuCau = await _dbContext.Set<NhanSuYeuCau>()
            .AnyAsync(x => x.NhanVienId == nhanVienId, cancellationToken);
            
        if (hasYeuCau) return true;
        
        return await _dbContext.NhanSuBaoTris
            .AnyAsync(x => x.NhanVienId == nhanVienId, cancellationToken);
    }

    public async Task<bool> MaNhanVienExistsAsync(string maNhanVien, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NhanViens
            .AnyAsync(x => x.MaNhanVien == maNhanVien, cancellationToken);
    }

    public async Task<NhanVien?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NhanViens
            .FirstOrDefaultAsync(x => x.NguoiDungId == userId, cancellationToken);
    }
}
