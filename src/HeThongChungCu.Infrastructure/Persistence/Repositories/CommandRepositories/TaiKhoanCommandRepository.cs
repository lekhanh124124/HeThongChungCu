using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class TaiKhoanCommandRepository : ITaiKhoanCommandRepository
{
    private readonly AppDbContext _dbContext;

    public TaiKhoanCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaiKhoan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TaiKhoan?> GetWithAvatarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.AnhDaiDien)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TaiKhoan?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.PhanQuyens)
            .FirstOrDefaultAsync(x => x.Email.Value == email, cancellationToken);
    }

    public async Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.PhanQuyens)
            .Include(a => a.AnhDaiDien)
            .FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap, cancellationToken);
    }

    public async Task<TaiKhoan?> GetWithRolesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.PhanQuyens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TaiKhoan?> GetWithRolesAndTokensAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.PhanQuyens)
            .Include(a => a.Tokens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TaiKhoan?> GetWithTokensAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.Tokens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TaiKhoan?> GetByNguoiDungIdAsync(int nguoiDungId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.PhanQuyens)
            .Include(a => a.Tokens)
            .FirstOrDefaultAsync(x => x.NguoiDungId == nguoiDungId, cancellationToken);
    }

    public async Task<TaiKhoan?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.PhanQuyens)
            .Include(a => a.Tokens)
            .Include(a => a.AnhDaiDien)
            .FirstOrDefaultAsync(a => a.Tokens.Any(rt => rt.TokenHash == refreshTokenHash), cancellationToken);
    }

    public async Task<TaiKhoan?> GetByTokenAsync(string tokenHash, TokenType tokenType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Include(a => a.PhanQuyens)
            .Include(a => a.Tokens)
            .FirstOrDefaultAsync(a => a.Tokens.Any(t => t.TokenHash == tokenHash && t.TokenType == tokenType), cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TaiKhoan, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan.AnyAsync(expression, cancellationToken);
    }

    public async Task<IEnumerable<TaiKhoan>> GetByNguoiDungIdsAsync(IEnumerable<int> nguoiDungIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Where(t => t.NguoiDungId.HasValue && nguoiDungIds.Contains(t.NguoiDungId.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TaiKhoan taiKhoan, CancellationToken cancellationToken = default)
    {
        await _dbContext.TaiKhoan.AddAsync(taiKhoan, cancellationToken);
    }

    public void Update(TaiKhoan taiKhoan)
    {
        _dbContext.TaiKhoan.Update(taiKhoan);
    }

    public void Delete(TaiKhoan taiKhoan)
    {
        _dbContext.TaiKhoan.Remove(taiKhoan);
    }

    public async Task<List<int>> GetNguoiDungIdsByRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaiKhoan
            .Where(tk => tk.PhanQuyens.Any(pq => pq.RoleId == role) && tk.NguoiDungId.HasValue)
            .Select(tk => tk.NguoiDungId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
