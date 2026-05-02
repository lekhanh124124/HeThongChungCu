namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

public class ExportFileResponse
{
    public byte[] Data { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
