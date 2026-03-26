using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class TepTaiLieu : AuditableEntity
{
    public string FileName { get; private set; } = null!;
    public string FileUrl { get; private set; } = null!;
    public long Size { get; private set; }
    public string ContentType { get; private set; } = null!;
    public bool IsUsed { get; private set; }

    private TepTaiLieu() { } // EF Core

    public TepTaiLieu(string fileName, string fileUrl, long size, string contentType)
    {
        FileName = fileName;
        FileUrl = fileUrl;
        Size = size;
        ContentType = contentType;
        IsUsed = false;
    }

    public TepTaiLieu(string fileName, string fileUrl, string contentType)
    {
        FileName = fileName;
        FileUrl = fileUrl;
        Size = 0;
        ContentType = contentType;
        IsUsed = false;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }

    public void MarkAsUnused()
    {
        IsUsed = false;
    }
}
