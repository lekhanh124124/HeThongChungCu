namespace HeThongChungCu.Domain.Common;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new(
        "Error.NullValue",
        "Giá trị là null.");

    public static Error FromException(Exception exception) => new(
        "Error.Exception",
        exception.Message);

    public static implicit operator string(Error error) => error.Code;

    public override string ToString() => Code;
}
