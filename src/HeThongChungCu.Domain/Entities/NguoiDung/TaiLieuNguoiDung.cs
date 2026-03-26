using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TaiLieuNguoiDung : BaseEntity
{
    public int? NguoiDungId { get; private set; }
    public NguoiDung? NguoiDung { get; private set; }
    public LoaiGiayTo LoaiGiayToId { get; private set; } = null!;
    public string SoGiayTo { get; private set; } = null!;
    public DateTime? NgayPhatHanh { get; private set; }
    
    private readonly List<TepTaiLieu> _files = [];
    public IReadOnlyCollection<TepTaiLieu> Files => _files.AsReadOnly();

    private TaiLieuNguoiDung() { } // EF Core

    public TaiLieuNguoiDung(int? nguoiDungId, LoaiGiayTo loaiGiayTo, string soGiayTo, DateTime? ngayPhatHanh, IEnumerable<TepTaiLieu>? files = null)
    {
        NguoiDungId = nguoiDungId;
        LoaiGiayToId = loaiGiayTo;
        SoGiayTo = soGiayTo;
        NgayPhatHanh = ngayPhatHanh;
        if (files != null)
        {
            foreach (var file in files)
            {
                file.MarkAsUsed();
                _files.Add(file);
            }
        }
    }

    public void LinkToUser(int nguoiDungId)
    {
        NguoiDungId = nguoiDungId;
    }
}
