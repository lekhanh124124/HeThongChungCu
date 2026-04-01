namespace HeThongChungCu.Infrastructure.Common.Settings;

public class PersistenceSettings
{
    public const string SectionName = "ConnectionStrings";

    public string DefaultConnection { get; init; } = null!;
}
