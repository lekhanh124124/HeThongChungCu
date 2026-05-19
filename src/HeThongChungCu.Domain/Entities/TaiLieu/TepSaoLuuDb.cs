using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TepSaoLuuDb : TepTaiLieu
{
    private TepSaoLuuDb() : base() { }

    public TepSaoLuuDb(
        string fileName,
        string fileUrl,
        long size,
        string contentType)
        : base(LoaiTepTaiLieu.SaoLuuDb, fileName, fileUrl, size, contentType)
    {
    }
}
