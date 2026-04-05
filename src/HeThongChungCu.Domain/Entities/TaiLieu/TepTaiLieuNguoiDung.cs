namespace HeThongChungCu.Domain.Entities;

public class TepTaiLieuNguoiDung : TepTaiLieu
{
    public int TaiLieuNguoiDungId { get; private set; }
    public TaiLieuNguoiDung TaiLieuNguoiDung { get; private set; } = null!;

    private TepTaiLieuNguoiDung() { }

    public TepTaiLieuNguoiDung(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int taiLieuNguoiDungId = 0)
        : base(fileName, fileUrl, size, contentType)
    {
        TaiLieuNguoiDungId = taiLieuNguoiDungId;
    }
}
