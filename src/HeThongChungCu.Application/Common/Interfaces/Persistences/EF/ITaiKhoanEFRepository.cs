using System.Linq.Expressions;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface ITaiKhoanEFRepository
{
    Task<TaiKhoan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetWithAvatarAsync(int id, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetWithRolesAsync(int id, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetWithRolesAndTokensAsync(int id, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetWithTokensAsync(int id, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetByNguoiDungIdAsync(int nguoiDungId, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetByRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<TaiKhoan?> GetByTokenAsync(string tokenHash, TokenType tokenType, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TaiKhoan, bool>> expression, CancellationToken cancellationToken = default);
    Task<List<int>> GetNguoiDungIdsByRoleAsync(Role role, CancellationToken cancellationToken = default);

    Task AddAsync(TaiKhoan taiKhoan, CancellationToken cancellationToken = default);
    void Update(TaiKhoan taiKhoan);
    void Delete(TaiKhoan taiKhoan);
}
