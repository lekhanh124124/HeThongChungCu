namespace HeThongChungCu.Domain.Entities;

public class TepYeuCauThiCongNoiThat : TepTaiLieu
{
    public int YeuCauThiCongNoiThatId { get; private set; }
    public YeuCauThiCongNoiThat YeuCauThiCongNoiThat { get; private set; } = null!;

    private TepYeuCauThiCongNoiThat() { }

    public TepYeuCauThiCongNoiThat(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int yeuCauThiCongNoiThatId = 0)
        : base(fileName, fileUrl, size, contentType)
    {
        YeuCauThiCongNoiThatId = yeuCauThiCongNoiThatId;
    }
}
