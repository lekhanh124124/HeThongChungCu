namespace HeThongChungCu.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorCode { get; protected set; }

    protected DomainException(string message, string errorCode = "Domain.Error")
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
