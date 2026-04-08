using HeThongChungCu.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace HeThongChungCu.Domain.ValueObjects;

public record SoDienThoai
{
    public string Value { get; }

    private static readonly Regex PhoneRegex = new(@"^((0[23456789]\d{8,9})|(1[89]00\d{4,6}))$", RegexOptions.Compiled);

    public SoDienThoai(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = string.Empty;
            return;
        }

        var cleanedValue = value.Replace(" ", "").Replace(".", "").Replace("-", "");
        
        if (!PhoneRegex.IsMatch(cleanedValue))
        {
            throw new BusinessException($"Số điện thoại '{value}' không hợp lệ.");
        }

        Value = cleanedValue;
    }

    public static implicit operator string(SoDienThoai? phone) => phone?.Value ?? string.Empty;
    public static implicit operator SoDienThoai(string? value) => new(value);

    public override string ToString() => Value;
}
