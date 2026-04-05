using HeThongChungCu.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace HeThongChungCu.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public Email(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = string.Empty;
            return;
        }

        if (!EmailRegex.IsMatch(value))
        {
            throw new BusinessException($"Email '{value}' không hợp lệ.");
        }

        Value = value.ToLowerInvariant();
    }

    public static implicit operator string(Email? email) => email?.Value ?? string.Empty;
    public static implicit operator Email(string? value) => new(value);

    public override string ToString() => Value;
}
