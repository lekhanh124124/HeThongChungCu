using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TepTaiLieu : AuditableEntity
{
    public LoaiTepTaiLieu LoaiTepId { get; protected set; } = null!;
    public string FileName { get; protected set; } = null!;
    public string FileUrl { get; protected set; } = null!;
    public long Size { get; protected set; }
    public string ContentType { get; protected set; } = null!;
    public bool IsUsed { get; protected set; }

    protected TepTaiLieu() { } // EF Core

    public TepTaiLieu(string fileName, string fileUrl, long size, string contentType)
    {
        LoaiTepId = LoaiTepTaiLieu.MacDinh;
        FileName = fileName;
        FileUrl = fileUrl;
        Size = size;
        ContentType = contentType;
        IsUsed = false;
    }

    protected TepTaiLieu(LoaiTepTaiLieu loaiTepId, string fileName, string fileUrl, long size, string contentType)
    {
        LoaiTepId = loaiTepId;
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
