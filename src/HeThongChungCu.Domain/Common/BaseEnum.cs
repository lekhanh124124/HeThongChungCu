using HeThongChungCu.Domain.Exceptions;
using System.Reflection;

namespace HeThongChungCu.Domain.Common;

public abstract class BaseEnum<TEnum, TValue> : IEquatable<BaseEnum<TEnum, TValue>>
    where TEnum : BaseEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>
{
    public TValue Value { get; init; }
    public string Name { get; init; }

    protected BaseEnum(TValue value, string name)
    {
        Value = value;
        Name = name;
    }

    private static readonly Lazy<Dictionary<TValue, TEnum>> _fromValue =
        new(() => GetAll().ToDictionary(e => e.Value));

    private static readonly Lazy<Dictionary<string, TEnum>> _fromName =
        new(() => GetAll().ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase));

    private static readonly Lazy<IReadOnlyDictionary<TValue, string>> _valueNameMap =
        new(() => GetAll().ToDictionary(e => e.Value, e => e.Name));

    private static readonly Lazy<IReadOnlyCollection<TEnum>> _all =
        new(() => typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<TEnum>()
            .ToList());

    public static IReadOnlyCollection<TEnum> GetAll()
        => _all.Value;

    public static IReadOnlyDictionary<TValue, string> ToDictionary()
        => _valueNameMap.Value;

    public static TEnum? FromValue(
        TValue value,
        Func<TValue, Exception>? exceptionFactory = null)
    {
        if (value is null)
        {
            if (exceptionFactory is not null)
                throw exceptionFactory(value!);
            throw new NotFoundException(typeof(TEnum).Name, value!);
        }
        _fromValue.Value.TryGetValue(value, out var matchingItem);
        return matchingItem;
    }

    public static TEnum? FromName(
        string name,
        Func<string, Exception>? exceptionFactory = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            if (exceptionFactory is not null)
                throw exceptionFactory(name);
            throw new NotFoundException(typeof(TEnum).Name, name);
        }
        _fromName.Value.TryGetValue(name, out var matchingItem);
        return matchingItem;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BaseEnum<TEnum, TValue>)obj);
    }

    public bool Equals(BaseEnum<TEnum, TValue>? other)
    {
        if (other is null) return false;
        return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(BaseEnum<TEnum, TValue>? left, BaseEnum<TEnum, TValue>? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(BaseEnum<TEnum, TValue>? left, BaseEnum<TEnum, TValue>? right)
    {
        return !(left == right);
    }

    public override string ToString() => Name;
}
