using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TepHopDongDoiTac : TepTaiLieu
{
    public int HopDongDoiTacId { get; private set; }
    public virtual HopDongDoiTac HopDongDoiTac { get; private set; } = null!;

    private TepHopDongDoiTac() : base() { }

    public TepHopDongDoiTac(
        string fileName,
        string fileUrl,
        long size,
        string contentType,
        int hopDongDoiTacId = 0)
        : base(LoaiTepTaiLieu.HopDongDoiTac, fileName, fileUrl, size, contentType)
    {
        HopDongDoiTacId = hopDongDoiTacId;
    }
}
