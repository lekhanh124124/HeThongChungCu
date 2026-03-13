using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class CanHo : AggregateRoot
{
    public string MaCanHo { get; private set; } = null!;
    public string TenCanHo { get; private set; } = null!;

    public decimal DienTich { get; private set; }
    public int SoPhongNgu { get; private set; }
    public int SoPhongTam { get; private set; }

    public LoaiCanHo LoaiCanHoId { get; private set; } = null!;
    public TinhTrangCanHo TinhTrangCanHoId { get; private set; } = null!;

    public int TangId { get; private set; }
    public Tang Tang { get; private set; } = null!;

    private readonly List<QuanHeCuTru> _quanHeCuTrus = new();
    public IReadOnlyCollection<QuanHeCuTru> QuanHeCuTrus => _quanHeCuTrus.AsReadOnly();

    private readonly List<ChiSoTieuThu> _chiSoTieuThus = new();
    public IReadOnlyCollection<ChiSoTieuThu> ChiSoTieuThus => _chiSoTieuThus.AsReadOnly();

    private CanHo() { } // EF Core

    public CanHo(string maCanHo, string tenCanHo, decimal dienTich, int tangId, int soPhongNgu, int soPhongTam, LoaiCanHo loaiCanHoId, TinhTrangCanHo tinhTrangCanHoId)
    {
        MaCanHo = maCanHo;
        TenCanHo = tenCanHo;
        DienTich = dienTich;
        TangId = tangId;
        SoPhongNgu = soPhongNgu;
        SoPhongTam = soPhongTam;
        LoaiCanHoId = loaiCanHoId;
        TinhTrangCanHoId = tinhTrangCanHoId;
    }

    public void UpdateInfo(string tenCanHo, decimal dienTich, int tangId, int soPhongNgu, int soPhongTam, LoaiCanHo loaiCanHoId)
    {
        TenCanHo = tenCanHo;
        DienTich = dienTich;
        TangId = tangId;
        SoPhongNgu = soPhongNgu;
        SoPhongTam = soPhongTam;
        LoaiCanHoId = loaiCanHoId;
    }

    public void UpdateStatus(TinhTrangCanHo tinhTrangCanHoId)
    {
        TinhTrangCanHoId = tinhTrangCanHoId;
    }

    public void AddQuanHeCuTru(int userId, LoaiQuanHeCuTru loaiQuanHeCuTruId, DateTime ngayBatDau)
    {
        var quanHe = new QuanHeCuTru(Id, userId, loaiQuanHeCuTruId, ngayBatDau);
        _quanHeCuTrus.Add(quanHe);
    }

    public void RemoveQuanHeCuTru(QuanHeCuTru quanHeCuTru)
    {
        _quanHeCuTrus.Remove(quanHeCuTru);
    }

    public void AddChiSoTieuThu(ChiSoTieuThu chiSo)
    {
        _chiSoTieuThus.Add(chiSo);
    }

    private readonly List<PhuongTien.PhuongTien> _phuongTiens = new();
    public IReadOnlyCollection<PhuongTien.PhuongTien> PhuongTiens => _phuongTiens.AsReadOnly();

    public void AddPhuongTien(PhuongTien.PhuongTien phuongTien)
    {
        _phuongTiens.Add(phuongTien);
    }
}
