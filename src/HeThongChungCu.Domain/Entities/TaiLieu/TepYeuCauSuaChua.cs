using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TepYeuCauSuaChua : TepTaiLieu
{
    public int YeuCauSuaChuaId { get; private set; }
    public YeuCauSuaChua YeuCauSuaChua { get; private set; } = null!;

    private TepYeuCauSuaChua() : base() { }

    public TepYeuCauSuaChua(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int yeuCauSuaChuaId = 0)
        : base(LoaiTepTaiLieu.YeuCauSuaChua, fileName, fileUrl, size, contentType)
    {
        YeuCauSuaChuaId = yeuCauSuaChuaId;
    }
}
