using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TepPhuongTien : TepTaiLieu
{
    public int PhuongTienId { get; private set; }
    public PhuongTien PhuongTien { get; private set; } = null!;

    private TepPhuongTien() : base() { }

    public TepPhuongTien(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int phuongTienId = 0)
        : base(LoaiTepTaiLieu.PhuongTien, fileName, fileUrl, size, contentType)
    {
        PhuongTienId = phuongTienId;
    }
}
