namespace HeThongChungCu.Domain.Entities;

public class TepYeuCauTaiLieuCuTru : TepTaiLieu
{
    public int YeuCauTaiLieuCuTruId { get; private set; }
    public YeuCauTaiLieuCuTru YeuCauTaiLieuCuTru { get; private set; } = null!;

    private TepYeuCauTaiLieuCuTru() { }

    public TepYeuCauTaiLieuCuTru(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int yeuCauTaiLieuCuTruId = 0)
        : base(fileName, fileUrl, size, contentType)
    {
        YeuCauTaiLieuCuTruId = yeuCauTaiLieuCuTruId;
    }
}
