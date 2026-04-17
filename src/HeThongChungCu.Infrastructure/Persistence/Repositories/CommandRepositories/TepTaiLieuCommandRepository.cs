using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class TepTaiLieuCommandRepository : ITepTaiLieuCommandRepository
{
    private readonly AppDbContext _context;

    public TepTaiLieuCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TepTaiLieu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.TepTaiLieus.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<TepTaiLieu>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.TepTaiLieus
            .Where(f => ids.Contains(f.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TepTaiLieu>> GetUnusedFilesAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        // Tìm tệp chưa được sử dụng và tạo trước thời điểm before
        return await _context.TepTaiLieus
            .Where(f => !f.IsUsed && f.CreatedAt <= before)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TepTaiLieu file, CancellationToken cancellationToken = default)
    {
        await _context.TepTaiLieus.AddAsync(file, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TepTaiLieu> files, CancellationToken cancellationToken = default)
    {
        await _context.TepTaiLieus.AddRangeAsync(files, cancellationToken);
    }

    public void Update(TepTaiLieu file)
    {
        _context.TepTaiLieus.Update(file);
    }

    public void Delete(TepTaiLieu file)
    {
        _context.TepTaiLieus.Remove(file);
    }

    public void DeleteRange(IEnumerable<TepTaiLieu> files)
    {
        _context.TepTaiLieus.RemoveRange(files);
    }
}
