using System.Linq.Expressions;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF
{
    public interface IUserEFRepository
    {
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetByIdCardAsync(string idCard, CancellationToken cancellationToken = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<User, bool>> expression, CancellationToken cancellationToken = default);

        Task AddAsync(User user, CancellationToken cancellationToken = default);
        void Update(User user);
        void Delete(User user);
    }

}
