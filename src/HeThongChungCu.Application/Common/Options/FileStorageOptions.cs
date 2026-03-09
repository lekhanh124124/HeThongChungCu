namespace HeThongChungCu.Application.Common.Options;

public class FileStorageOptions
{
    public const string SectionName = "FileStorageSettings";

    public string ConnectionString { get; set; } = string.Empty;
    public string UserAvatarContainer { get; set; } = string.Empty;
    public string BuildingContainer { get; set; } = string.Empty;
    public string ApartmentContainer { get; set; } = string.Empty;
}
