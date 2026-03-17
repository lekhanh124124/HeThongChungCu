namespace HeThongChungCu.Domain.Exceptions
{
    public class BusinessException : DomainException
    {
        public BusinessException(string message, string code = "Business.Error")
            : base(message, code)
        {
        }
    }
}
