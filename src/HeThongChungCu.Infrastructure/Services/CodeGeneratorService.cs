using System.Linq.Expressions;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
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

    public async Task<string> GenerateMaToaNhaAsync(CancellationToken cancellationToken = default)
    {
        // Suggest TN{Max+1:D2}
        var codes = await _context.Set<ToaNha>()
            .Select(t => t.MaToaNha)
            .ToListAsync(cancellationToken);

        var maxNumber = codes
            .Select(c => {
                if (c.StartsWith("TN") && int.TryParse(c.Substring(2), out var n))
                    return n;
                return 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        return $"TN{(maxNumber + 1):D2}";
    }

    public async Task<string> GenerateMaTangAsync(int toaNhaId, int loaiTangValue, CancellationToken cancellationToken = default)
    {
        var toaNha = await _context.Set<ToaNha>()
            .Include(t => t.Tangs)
            .FirstOrDefaultAsync(t => t.Id == toaNhaId, cancellationToken);

        if (toaNha == null) return string.Empty;

        var loaiTang = LoaiTang.FromValue(loaiTangValue);
        if (loaiTang == null) return string.Empty;

        var prefix = loaiTang == LoaiTang.TangHam ? "B" : "F";
        
        var maxNumber = toaNha.Tangs
            .Where(t => t.LoaiTangId == loaiTang)
            .Select(t => {
                var firstPart = t.MaTang.Split('-')[0]; // F1 or B1
                if (firstPart.Length > 1 && int.TryParse(firstPart.Substring(1), out var n))
                    return (int?)n;
                return (int?)0;
            })
            .DefaultIfEmpty(0)
            .Max() ?? 0;

        return $"{prefix}{maxNumber + 1}-{toaNha.MaToaNha}";
    }

    public async Task<string> GenerateMaCanHoAsync(int tangId, CancellationToken cancellationToken = default)
    {
        var toaNha = await _context.Set<ToaNha>()
            .Include(t => t.Tangs)
            .FirstOrDefaultAsync(t => t.Tangs.Any(tang => tang.Id == tangId), cancellationToken);

        if (toaNha == null) return string.Empty;

        var tang = toaNha.Tangs.First(t => t.Id == tangId);
        
        // Extract floor number from name (e.g., "Tầng 1" -> 1)
        int floorNum = 0;
        if (tang.TenTang.Contains(" "))
        {
            var parts = tang.TenTang.Split(' ');
            var lastPart = parts[^1];
            if (lastPart.StartsWith("B")) lastPart = lastPart.Substring(1);
            int.TryParse(lastPart, out floorNum);
        }

        // Get codes for this floor and process in-memory
        var codes = await _context.Set<CanHo>()
            .Where(c => c.TangId == tangId)
            .Select(c => c.MaCanHo)
            .ToListAsync(cancellationToken);

        var maxRoomNum = codes
            .Select(maCanHo => {
                var parts = maCanHo.Split('-'); // SKR-101
                if (parts.Length > 1)
                {
                    var roomPart = parts[1]; // 101
                    var floorStr = floorNum.ToString();
                    if (roomPart.StartsWith(floorStr) && roomPart.Length > floorStr.Length)
                    {
                        if (int.TryParse(roomPart.Substring(floorStr.Length), out var r))
                            return r;
                    }
                }
                return 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        return $"{toaNha.MaToaNha}-{floorNum}{(maxRoomNum + 1):D2}";
    }
}
