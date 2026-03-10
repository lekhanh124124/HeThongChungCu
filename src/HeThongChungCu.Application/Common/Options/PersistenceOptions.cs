namespace HeThongChungCu.Application.Common.Options;

public class PersistenceOptions
{
    public const string SectionName = "ConnectionStrings";

    public string DefaultConnection { get; init; } = null!;
}
