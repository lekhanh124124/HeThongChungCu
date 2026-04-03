using System.Linq.Expressions;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Services;

public class CodeGeneratorService : ICodeGeneratorService
{
    private readonly AppDbContext _context;

    public CodeGeneratorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync<TEntity>(
        string prefix, 
        Expression<Func<TEntity, string>> propertySelector, 
        int length = 4,
        bool includeYear = true) where TEntity : class
    {
        var yearPrefix = includeYear ? $"{DateTime.Now.Year}-" : "";
        var searchPrefix = $"{prefix}-{yearPrefix}";

        // Get the latest code from the database
        var lastCode = await _context.Set<TEntity>()
            .Select(propertySelector)
            .Where(c => c.StartsWith(searchPrefix))
            .OrderByDescending(c => c)
            .FirstOrDefaultAsync();

        int nextNumber = 1;

        if (!string.IsNullOrEmpty(lastCode))
        {
            var parts = lastCode.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[^1], out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        var formattedNumber = nextNumber.ToString().PadLeft(length, '0');
        return $"{searchPrefix}{formattedNumber}";
    }

    public string GenerateRandomPassword(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var randomBytes = new byte[length];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[randomBytes[i] % chars.Length];
        }

        return new string(result);
    }
}
