namespace HeThongChungCu.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorCode { get; protected set; }

    protected DomainException(string message, string errorCode = "DomainError") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
