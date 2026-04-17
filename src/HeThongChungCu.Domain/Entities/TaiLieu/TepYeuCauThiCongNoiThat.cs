using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TepYeuCauThiCongNoiThat : TepTaiLieu
{
    public int YeuCauThiCongNoiThatId { get; private set; }
    public YeuCauThiCongNoiThat YeuCauThiCongNoiThat { get; private set; } = null!;

    private TepYeuCauThiCongNoiThat() : base() { }

    public TepYeuCauThiCongNoiThat(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int yeuCauThiCongNoiThatId = 0)
        : base(LoaiTepTaiLieu.YeuCauThiCongNoiThat, fileName, fileUrl, size, contentType)
    {
        YeuCauThiCongNoiThatId = yeuCauThiCongNoiThatId;
    }
}
