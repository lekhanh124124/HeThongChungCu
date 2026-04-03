using System.Linq.Expressions;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface ICodeGeneratorService
{
    /// <summary>
    /// Generates a next incremental code based on prefix and existing records.
    /// Format: {prefix}-{YYYY}-{XXXX} or {prefix}-{XXXX}
    /// </summary>
    Task<string> GenerateAsync<TEntity>(
        string prefix, 
        Expression<Func<TEntity, string>> propertySelector, 
        int length = 4,
        bool includeYear = true) where TEntity : class;

    /// <summary>
    /// Generates a secure random password with standard complexity.
    /// </summary>
    string GenerateRandomPassword(int length = 8);
}
