using HeThongChungCu.Application.Common.Interfaces.Services;

namespace HeThongChungCu.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateTimeOffset Now => DateTimeOffset.Now;
}
