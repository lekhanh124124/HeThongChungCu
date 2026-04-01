namespace HeThongChungCu.Infrastructure.Common.Settings;

public sealed class ApplicationInsightsSettings
{
    public const string SectionName = "ApplicationInsights";

    public string ConnectionString { get; init; } = string.Empty;
}
