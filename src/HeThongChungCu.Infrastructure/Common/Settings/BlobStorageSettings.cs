namespace HeThongChungCu.Infrastructure.Common.Settings;

public class BlobStorageSettings
{
    public const string SectionName = "FileStorageSettings";

    public string ConnectionString { get; set; } = string.Empty;
    public string UserAvatarContainer { get; set; } = string.Empty;
    public string BuildingContainer { get; set; } = string.Empty;
    public string ApartmentContainer { get; set; } = string.Empty;
    public string DocumentContainer { get; set; } = string.Empty;
    public string VehicleContainer { get; set; } = string.Empty;
    public string StaffDocumentContainer { get; set; } = string.Empty;
    public string PartnerDocumentContainer { get; set; } = string.Empty;
}
