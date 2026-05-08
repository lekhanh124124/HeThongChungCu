using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TepYeuCauPhanAnh : TepTaiLieu
{
    public int YeuCauPhanAnhId { get; private set; }
    public YeuCauPhanAnh YeuCauPhanAnh { get; private set; } = null!;

    private TepYeuCauPhanAnh() : base() { }

    public TepYeuCauPhanAnh(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int yeuCauPhanAnhId = 0)
        : base(LoaiTepTaiLieu.YeuCauPhanAnh, fileName, fileUrl, size, contentType)
    {
        YeuCauPhanAnhId = yeuCauPhanAnhId;
    }
}
