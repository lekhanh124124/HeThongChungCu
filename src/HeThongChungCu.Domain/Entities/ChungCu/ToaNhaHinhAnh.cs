using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class ToaNhaHinhAnh : AuditableEntity
{
    public int ToaNhaId { get; private set; }
    public string HinhAnhUrl { get; private set; } = null!;
    public string? MoTa { get; private set; }
    public bool IsThumbnail { get; private set; }
    public int ThuTu { get; private set; }


    private ToaNhaHinhAnh() { } // EF Core

    public ToaNhaHinhAnh(int toaNhaId, string hinhAnhUrl, string? moTa, bool isThumbnail, int thuTu)
    {
        ToaNhaId = toaNhaId;
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
