namespace HeThongChungCu.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" ({key}) was not found.", "Domain.NotFound")
    {
    }
}
