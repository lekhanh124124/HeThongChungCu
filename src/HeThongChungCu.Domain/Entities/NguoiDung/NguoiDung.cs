using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class NguoiDung : AggregateRoot
{
    public string Ten { get; private set; } = string.Empty;
    public string Ho { get; private set; } = string.Empty;
    public string HoTen => $"{Ho} {Ten}";

    public DateTimeOffset NgaySinh { get; private set; }
    public GioiTinh GioiTinhId { get; private set; } = default!;
    public DiaChi DiaChi { get; private set; } = null!;
    public string? CCCD { get; private set; }
    public SoDienThoai? SoDienThoai { get; private set; }

    private readonly List<TaiLieuNguoiDung> _documents = [];
    public IReadOnlyCollection<TaiLieuNguoiDung> TaiLieu => _documents.AsReadOnly();

    private NguoiDung() { } // EF Core

    public NguoiDung(string ten, string ho, DateTimeOffset ngaySinh, GioiTinh gioiTinhId, string? diaChi, string? cccd = null, SoDienThoai? soDienThoai = null)
    {
        Ten = ten;
        Ho = ho;
        NgaySinh = ngaySinh;
        GioiTinhId = gioiTinhId;
        DiaChi = new DiaChi(diaChi);
        CCCD = cccd;
        SoDienThoai = soDienThoai;
    }

    public static NguoiDung CreateNguoiDung(
        string ten,
        string ho,
        DateTimeOffset ngaySinh,
        GioiTinh gioiTinhId,
        string? diaChi,
        string? cccd = null,
        SoDienThoai? soDienThoai = null,
        IEnumerable<TaiLieuNguoiDung>? documents = null)
    {
        var nguoiDung = new NguoiDung(ten, ho, ngaySinh, gioiTinhId, diaChi, cccd, soDienThoai);
        
        if (documents != null)
        {
            foreach (var doc in documents)
            {
                nguoiDung.AddDocument(doc);
            }
        }

        return nguoiDung;
    }

    public void UpdateProfile(string ten, string ho, DateTimeOffset ngaySinh, GioiTinh gioiTinhId, string? diaChi, string? cccd = null, SoDienThoai? soDienThoai = null)
    {
        Ten = ten;
        Ho = ho;
        NgaySinh = ngaySinh;
        GioiTinhId = gioiTinhId;
        DiaChi = new DiaChi(diaChi);
        CCCD = cccd;
        SoDienThoai = soDienThoai;
    }

    public void AddDocument(TaiLieuNguoiDung document)
    {
        _documents.Add(document);
    }

    public void RemoveDocument(int documentId)
    {
        var doc = _documents.FirstOrDefault(d => d.Id == documentId);
        if (doc != null)
        {
            _documents.Remove(doc);
        }
    }
}
