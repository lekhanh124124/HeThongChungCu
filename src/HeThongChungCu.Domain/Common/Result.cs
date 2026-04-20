namespace HeThongChungCu.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, Error[]? errors = null, string[]? warnings = null)
    {
        if (isSuccess && errors != null && errors.Length > 0 && errors.Any(e => e != Error.None))
        {
            throw new InvalidOperationException("Cannot create successful result with an error");
        }

        if (!isSuccess && (errors == null || errors.Length == 0))
        {
            throw new InvalidOperationException("Cannot create failed result without an error");
        }

        IsSuccess = isSuccess;
        Errors = errors ?? [];
        Warnings = warnings ?? [];
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[] Errors { get; }
    public string[] Warnings { get; }

    public bool IsOk => IsSuccess;

    public static Result Success() => new(true);
    public static Result Success(IEnumerable<string> warnings) => new(true, warnings: warnings.ToArray());

    public static Result Failure(Error error) => new(false, [error]);
    public static Result Failure(IEnumerable<Error> errors) => new(false, errors.ToArray());

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true);
    public static Result<TValue> Success<TValue>(TValue value, IEnumerable<string> warnings) => new(value, true, warnings: warnings.ToArray());

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, [error]);
    public static Result<TValue> Failure<TValue>(IEnumerable<Error> errors) => new(default, false, errors.ToArray());
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error[]? errors = null, string[]? warnings = null)
        : base(isSuccess, errors, warnings)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot access value of a failed result. Error(s): {string.Join(", ", Errors.Select(e => e.Code))}");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
    public static implicit operator Result<TValue>(Error[] errors) => Failure<TValue>(errors);
    public static implicit operator Result<TValue>(List<Error> errors) => Failure<TValue>(errors);
}
