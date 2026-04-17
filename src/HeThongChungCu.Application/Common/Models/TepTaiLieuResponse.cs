namespace HeThongChungCu.Application.Common.Models
{
    public record TepTaiLieuResponse(
        int Id,
        string FileUrl,
        string FileName,
        string ContentType
    );
}
