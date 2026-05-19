namespace HeThongChungCu.Application.Features.QLSystem.DTOs;

public class BackupHistoryResponse
{
    public int FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string ContentType { get; set; } = string.Empty;
}
