namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
    DateTimeOffset Now { get; }
}
