using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class DoiTac : AggregateRoot
{
    public string TenDoiTac { get; private set; } = string.Empty;
    public string? TenCongTy { get; private set; }
    public string? NguoiDaiDien { get; private set; }
    public string? SoGiayPhepKD { get; private set; }
    public string? MaSoThue { get; private set; }
    public DiaChi DiaChi { get; private set; } = null!;
    public SoDienThoai? SoDienThoai { get; private set; }
    public Email? Email { get; private set; }
    public string? GhiChu { get; private set; }

    private readonly List<HopDongDoiTac> _hopDongs = [];
    public virtual IReadOnlyCollection<HopDongDoiTac> HopDongs => _hopDongs.AsReadOnly();

    private DoiTac() { } // EF Core

    public DoiTac(
        string tenDoiTac,
        string? tenCongTy = null,
        string? nguoiDaiDien = null,
        string? soGiayPhepKD = null,
        string? maSoThue = null,
        string? diaChi = null,
        string? soDienThoai = null,
        string? email = null,
        string? ghiChu = null)
    {
        if (string.IsNullOrWhiteSpace(tenDoiTac))
            throw new BusinessException("Tên đối tác không được để trống.");

        TenDoiTac = tenDoiTac;
        TenCongTy = tenCongTy;
        NguoiDaiDien = nguoiDaiDien;
        SoGiayPhepKD = soGiayPhepKD;
        MaSoThue = maSoThue;
        DiaChi = new DiaChi(diaChi);
        SoDienThoai = new SoDienThoai(soDienThoai);
        Email = new Email(email);
        GhiChu = ghiChu;
    }

    public void UpdateInfo(
        string tenDoiTac,
        string? tenCongTy,
        string? nguoiDaiDien,
        string? soGiayPhepKD,
        string? maSoThue,
        string? diaChi,
        string? soDienThoai,
        string? email,
        string? ghiChu)
    {
        if (string.IsNullOrWhiteSpace(tenDoiTac))
            throw new BusinessException("Tên đối tác không được để trống.");

        TenDoiTac = tenDoiTac;
        TenCongTy = tenCongTy;
        NguoiDaiDien = nguoiDaiDien;
        SoGiayPhepKD = soGiayPhepKD;
        MaSoThue = maSoThue;
        DiaChi = new DiaChi(diaChi);
        SoDienThoai = new SoDienThoai(soDienThoai);
        Email = new Email(email);
        GhiChu = ghiChu;
    }

    public HopDongDoiTac KyHopDongMoi(
        string soHopDong,
        DateTimeOffset ngayKy,
        DateTimeOffset ngayHetHan,
        decimal giaTri,
        int dichVuId,
        string? noiDung = null)
    {
        var hopDong = new HopDongDoiTac(Id, soHopDong, ngayKy, ngayHetHan, giaTri, dichVuId, noiDung);
        _hopDongs.Add(hopDong);

        return hopDong;
    }

    public void AddHopDong(HopDongDoiTac hopDong)
    {
        _hopDongs.Add(hopDong);
    }

    public void RemoveHopDong(int hopDongId)
    {
        var hd = _hopDongs.FirstOrDefault(h => h.Id == hopDongId);
        if (hd != null)
        {
            _hopDongs.Remove(hd);
        }
    }

    public void CheckActiveHopDongs()
    {
        foreach (var hopDong in _hopDongs)
        {
            hopDong.UpdateStatus();
        }

        if (!_hopDongs.Any(h => h.IsActive()))
        {
            AddDomainEvent(new HeThongChungCu.Domain.Events.DoiTacHopDongHetHanEvent(Id));
        }
    }
}
