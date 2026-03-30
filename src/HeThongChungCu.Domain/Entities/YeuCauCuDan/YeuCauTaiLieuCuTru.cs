using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauTaiLieuCuTru : BaseEntity
{
    public int YeuCauCuTruId { get; private set; }
    public YeuCauCuTru YeuCauCuTru { get; private set; } = null!;

    public LoaiGiayTo LoaiGiayToId { get; private set; } = null!;
    public string SoGiayTo { get; private set; } = null!;
    public DateTime? NgayPhatHanh { get; private set; }

    public int? TaiLieuCuTruId { get; private set; }

    private readonly List<TepTaiLieu> _files = [];
    public IReadOnlyCollection<TepTaiLieu> Files => _files.AsReadOnly();

    private YeuCauTaiLieuCuTru() { } // EF Core

    public YeuCauTaiLieuCuTru(
        LoaiGiayTo loaiGiayTo,
        string soGiayTo,
        DateTime? ngayPhatHanh,
        IEnumerable<TepTaiLieu>? files = null,
        int? taiLieuCuTruId = null)
    {
        LoaiGiayToId = loaiGiayTo;
        SoGiayTo = soGiayTo;
        NgayPhatHanh = ngayPhatHanh;
        TaiLieuCuTruId = taiLieuCuTruId;
        if (files != null)
        {
            foreach (var file in files)
            {
                file.MarkAsUsed();
                _files.Add(file);
            }
        }
    }
}
