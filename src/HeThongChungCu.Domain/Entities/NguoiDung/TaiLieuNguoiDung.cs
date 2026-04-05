using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class TaiLieuNguoiDung : TaiLieu
{
    public int? NguoiDungId { get; private set; }
    public NguoiDung? NguoiDung { get; private set; }
    
    private readonly List<TepTaiLieuNguoiDung> _files = [];
    public IReadOnlyCollection<TepTaiLieuNguoiDung> Files => _files.AsReadOnly();

    private TaiLieuNguoiDung() { } // EF Core

    public TaiLieuNguoiDung(int? nguoiDungId, LoaiGiayTo loaiGiayTo, string soGiayTo, DateTimeOffset? ngayPhatHanh, IEnumerable<TepTaiLieuNguoiDung>? files = null)
        : base(loaiGiayTo, soGiayTo, ngayPhatHanh)
    {
        NguoiDungId = nguoiDungId;
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

    public void UpdateInfo(LoaiGiayTo loaiGiayTo, string soGiayTo, DateTimeOffset? ngayPhatHanh)
    {
        LoaiGiayToId = loaiGiayTo;
        SoGiayTo = soGiayTo;
        NgayPhatHanh = ngayPhatHanh;
    }

    public void SyncFiles(IEnumerable<TepTaiLieuNguoiDung>? files)
    {
        foreach (var file in _files)
        {
            file.MarkAsUnused();
        }
        _files.Clear();
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
