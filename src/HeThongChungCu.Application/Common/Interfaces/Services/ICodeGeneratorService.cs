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

    /// <summary>
    /// Generates the next building code (e.g., TN01).
    /// </summary>
    Task<string> GenerateMaToaNhaAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates the next floor code (e.g., F1-SKR, B1-SKR).
    /// </summary>
    Task<string> GenerateMaTangAsync(int toaNhaId, int loaiTangId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates the next apartment code (e.g., SKR-101).
    /// </summary>
    Task<string> GenerateMaCanHoAsync(int tangId, CancellationToken cancellationToken = default);
}
