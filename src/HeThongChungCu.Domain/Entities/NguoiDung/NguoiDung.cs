using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class NguoiDung : AggregateRoot
{
    public string Ten { get; private set; } = string.Empty;
    public string Ho { get; private set; } = string.Empty;
    public string HoTen => $"{Ho} {Ten}";

    public DateTime NgaySinh { get; private set; }
    public GioiTinh GioiTinhId { get; private set; } = null!;
    public string? DiaChi { get; private set; }
    public string? CCCD { get; private set; }
    public string? SoDienThoai { get; private set; }

    private readonly List<TaiLieuNguoiDung> _documents = [];
    public IReadOnlyCollection<TaiLieuNguoiDung> TaiLieu => _documents.AsReadOnly();

    private NguoiDung() { } // EF Core

    public NguoiDung(string ten, string ho, DateTime ngaySinh, GioiTinh gioiTinhId, string? diaChi, string? cccd = null, string? soDienThoai = null)
    {
        Ten = ten;
        Ho = ho;
        NgaySinh = ngaySinh;
        GioiTinhId = gioiTinhId;
        DiaChi = diaChi;
        CCCD = cccd;
        SoDienThoai = soDienThoai;
    }

    public void UpdateProfile(string ten, string ho, DateTime ngaySinh, GioiTinh gioiTinhId, string? diaChi, string? cccd = null, string? soDienThoai = null)
    {
        Ten = ten;
        Ho = ho;
        NgaySinh = ngaySinh;
        GioiTinhId = gioiTinhId;
        DiaChi = diaChi;
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
