namespace HeThongChungCu.Application.Common.Options;

public sealed class ApplicationInsightsOptions
{
    public const string SectionName = "ApplicationInsights";

    public string ConnectionString { get; init; } = string.Empty;
}
