using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class CanHoHinhAnh : AuditableEntity
{
    public int CanHoId { get; private set; }
    public string HinhAnhUrl { get; private set; } = null!;
    public string? MoTa { get; private set; }
    public bool IsThumbnail { get; private set; }
    public int ThuTu { get; private set; }

    private CanHoHinhAnh() { } // EF Core

    public CanHoHinhAnh(int canHoId, string hinhAnhUrl, string? moTa, bool isThumbnail, int thuTu)
    {
        CanHoId = canHoId;
        HinhAnhUrl = hinhAnhUrl;
        MoTa = moTa;
        IsThumbnail = isThumbnail;
        ThuTu = thuTu;
    }

    public void Update(string hinhAnhUrl, string? moTa, bool isThumbnail, int thuTu)
    {
        HinhAnhUrl = hinhAnhUrl;
        MoTa = moTa;
        IsThumbnail = isThumbnail;
        ThuTu = thuTu;
    }
}
