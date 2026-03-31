using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class ThongBao : AggregateRoot
{
    public string TieuDe { get; private set; } = null!;
    public string NoiDung { get; private set; } = null!;
    public LoaiThongBao LoaiThongBao { get; private set; } = null!;
    public string? ReferenceId { get; private set; }
    public string? Metadata { get; private set; } // JSON metadata

    private readonly List<PhanBoThongBao> _phanBoThongBaos = [];
    public IReadOnlyCollection<PhanBoThongBao> PhanBoThongBaos => _phanBoThongBaos.AsReadOnly();

    private ThongBao() { } // EF Core

    public ThongBao(string tieuDe, string noiDung, LoaiThongBao loaiThongBao, string? referenceId = null, string? metadata = null)
    {
        TieuDe = tieuDe;
        NoiDung = noiDung;
        LoaiThongBao = loaiThongBao;
        ReferenceId = referenceId;
        Metadata = metadata;
    }

    public void ThemPhanBo(int nguoiDungId)
    {
        if (!_phanBoThongBaos.Any(p => p.NguoiDungId == nguoiDungId))
        {
            _phanBoThongBaos.Add(new PhanBoThongBao(Id, nguoiDungId));
        }
    }
}
