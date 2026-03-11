using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

public class UserEFRepository : IUserEFRepository
{
    private readonly AppDbContext _dbContext;

    public UserEFRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
            .Include(u => u.Tokens)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
            .Include(u => u.Tokens)
            .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public async Task<User?> GetByIdCardAsync(string idCard, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.IdCard == idCard, cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
            .Include(u => u.Tokens)
            .FirstOrDefaultAsync(u => u.Tokens.Any(rt => rt.RefreshToken == refreshToken), cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<User, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>().AnyAsync(expression, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<User>().AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _dbContext.Set<User>().Update(user);
    }

    public void Delete(User user)
    {
        _dbContext.Set<User>().Remove(user);
    }
}
