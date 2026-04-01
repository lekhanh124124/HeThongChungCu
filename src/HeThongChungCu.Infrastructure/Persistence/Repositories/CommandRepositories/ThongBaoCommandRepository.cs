using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class ThongBaoCommandRepository : IThongBaoCommandRepository
{
    private readonly AppDbContext _context;

    public ThongBaoCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PhanBoThongBao?> GetPhanBoByIdAsync(int phanBoId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.PhanBoThongBaos
            .FirstOrDefaultAsync(p => p.Id == phanBoId && p.NguoiDungId == userId, cancellationToken);
    }

    public async Task AddAsync(ThongBao thongBao, CancellationToken cancellationToken = default)
    {
        await _context.ThongBaos.AddAsync(thongBao, cancellationToken);
    }

    public void UpdatePhanBo(PhanBoThongBao phanBo)
    {
        _context.PhanBoThongBaos.Update(phanBo);
    }
}
