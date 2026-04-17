using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class YeuCauTaiLieuCuTru : TaiLieu
{
    public int YeuCauCuTruId { get; private set; }
    public YeuCauCuTru YeuCauCuTru { get; private set; } = null!;

    public int? TaiLieuCuTruId { get; private set; }

    private readonly List<TepYeuCauTaiLieuCuTru> _files = [];
    public IReadOnlyCollection<TepYeuCauTaiLieuCuTru> Files => _files.AsReadOnly();

    private YeuCauTaiLieuCuTru() : base() { } // EF Core

    internal YeuCauTaiLieuCuTru(
        LoaiGiayTo loaiGiayTo,
        string soGiayTo,
        DateTimeOffset? ngayPhatHanh,
        IEnumerable<TepYeuCauTaiLieuCuTru>? files = null,
        int? taiLieuCuTruId = null)
        : base(LoaiTaiLieu.YeuCauCuTru, loaiGiayTo, soGiayTo, ngayPhatHanh)
    {
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
