namespace HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

public class TepYeuCauThiCongResponse
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public TepYeuCauThiCongResponse() { }
    public TepYeuCauThiCongResponse(int id, string fileUrl, string fileName, string contentType)
    {
        Id = id;
        FileUrl = fileUrl;
        FileName = fileName;
        ContentType = contentType;
    }
}
