using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class TriThucChatbotCommandRepository : ITriThucChatbotCommandRepository
{
    private readonly AppDbContext _context;

    public TriThucChatbotCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TriThucChatbot?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.TriThucChatbots
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<TriThucChatbot>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TriThucChatbots
            .Where(x => x.IsActive)
            .OrderBy(x => x.DanhMuc)
            .ThenBy(x => x.ThuTuHienThi)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TriThucChatbot>> GetByDanhMucAsync(string danhMuc, CancellationToken cancellationToken = default)
    {
        return await _context.TriThucChatbots
            .Where(x => x.IsActive && x.DanhMuc == danhMuc)
            .OrderBy(x => x.ThuTuHienThi)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TriThucChatbot>> GetSyncedInactiveAsync(CancellationToken cancellationToken = default)
    {
        // Bản ghi inactive nhưng đã từng sync → có vector trong Qdrant cần xóa
        return await _context.TriThucChatbots
            .Where(x => !x.IsActive && x.IsSynced)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public void Add(TriThucChatbot triThuc)
    {
        _context.TriThucChatbots.Add(triThuc);
    }

    public void Update(TriThucChatbot triThuc)
    {
        _context.TriThucChatbots.Update(triThuc);
    }

    public void Remove(TriThucChatbot triThuc)
    {
        _context.TriThucChatbots.Remove(triThuc);
    }
}
