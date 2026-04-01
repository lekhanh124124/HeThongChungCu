namespace HeThongChungCu.Infrastructure.Common.Settings;

public class FileCleanupSettings
{
    public const string SectionName = "FileStorageSettings";

    public int CleanupIntervalHours { get; set; } = 1;
    public int UnusedFileThresholdHours { get; set; } = 1;
}
