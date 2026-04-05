using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class TepTaiLieu : AuditableEntity
{
    public string FileName { get; protected set; } = null!;
    public string FileUrl { get; protected set; } = null!;
    public long Size { get; protected set; }
    public string ContentType { get; protected set; } = null!;
    public bool IsUsed { get; protected set; }

    protected TepTaiLieu() { } // EF Core

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
