namespace HeThongChungCu.Domain.Entities;

public class TepYeuCauPhuongTien : TepTaiLieu
{
    public int YeuCauPhuongTienId { get; private set; }
    public YeuCauPhuongTien YeuCauPhuongTien { get; private set; } = null!;

    private TepYeuCauPhuongTien() { }

    public TepYeuCauPhuongTien(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int yeuCauPhuongTienId = 0)
        : base(fileName, fileUrl, size, contentType)
    {
        YeuCauPhuongTienId = yeuCauPhuongTienId;
    }
}
