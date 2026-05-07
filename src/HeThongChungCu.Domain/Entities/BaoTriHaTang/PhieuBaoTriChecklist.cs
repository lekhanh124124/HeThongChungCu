using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class PhieuBaoTriChecklist : AuditableEntity
{
    public int PhieuBaoTriId { get; private set; }
    public string NoiDungChecklist { get; private set; } = null!;
    public bool? DatYeuCau { get; private set; }
    public string? GhiChuThucTe { get; private set; }
    public int? AnhMinhHoaId { get; private set; }
    public TepTaiLieu? AnhMinhHoa { get; private set; }

    private PhieuBaoTriChecklist() : base() { } // EF Core

    private PhieuBaoTriChecklist(string noiDungChecklist) : base()
    {
        NoiDungChecklist = noiDungChecklist;
        DatYeuCau = null; // Chưa làm
    }

    public static PhieuBaoTriChecklist Create(string noiDungChecklist)
    {
        return new PhieuBaoTriChecklist(noiDungChecklist);
    }

    public void UpdateResult(bool datYeuCau, string? ghiChuThucTe, int? anhMinhHoaId)
    {
        DatYeuCau = datYeuCau;
        GhiChuThucTe = ghiChuThucTe;
        if (anhMinhHoaId.HasValue)
        {
            AnhMinhHoaId = anhMinhHoaId;
        }
    }
}
