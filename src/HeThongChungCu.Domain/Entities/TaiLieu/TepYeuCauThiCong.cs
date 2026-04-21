using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TepYeuCauThiCong : TepTaiLieu
{
    public int YeuCauThiCongId { get; private set; }
    public YeuCauThiCong YeuCauThiCong { get; private set; } = null!;

    private TepYeuCauThiCong() : base() { }

    public TepYeuCauThiCong(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int yeuCauThiCongId = 0)
        : base(LoaiTepTaiLieu.YeuCauThiCong, fileName, fileUrl, size, contentType)
    {
        YeuCauThiCongId = yeuCauThiCongId;
    }
}
