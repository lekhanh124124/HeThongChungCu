namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class TepYeuCauPhanAnhReadModel
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}
