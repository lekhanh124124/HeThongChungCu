namespace HeThongChungCu.Application.Common.Models;

public class FileUploadItem
{
    public Stream Content { get; set; } = default!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
}
