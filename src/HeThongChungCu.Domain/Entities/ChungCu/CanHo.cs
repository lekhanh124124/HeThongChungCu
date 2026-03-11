using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities.ChungCu;

public class CanHo : AggregateRoot
{
    public string MaCanHo { get; private set; } = null!;

    public decimal DienTich { get; private set; }
    public int SoPhongNgu { get; private set; }
    public int SoPhongTam { get; private set; }

    public int LoaiCanHoId { get; private set; }
    public int TinhTrangCanHoId { get; private set; }

    public int TangId { get; private set; }
    public Tang Tang { get; private set; } = null!;

    private readonly List<QuanHeCuTru> _quanHeCuTrus = new();
    public IReadOnlyCollection<QuanHeCuTru> QuanHeCuTrus => _quanHeCuTrus.AsReadOnly();

    private readonly List<ChiSoTieuThu> _chiSoTieuThus = new();
    public IReadOnlyCollection<ChiSoTieuThu> ChiSoTieuThus => _chiSoTieuThus.AsReadOnly();

    private CanHo() { } // EF Core

    public CanHo(string maCanHo, decimal dienTich, int tangId, int soPhongNgu, int soPhongTam, int loaiCanHoId, int tinhTrangCanHoId)
    {
        MaCanHo = maCanHo;
        DienTich = dienTich;
        TangId = tangId;
        SoPhongNgu = soPhongNgu;
        SoPhongTam = soPhongTam;
        LoaiCanHoId = loaiCanHoId;
        TinhTrangCanHoId = tinhTrangCanHoId;
    }

    public void UpdateInfo(decimal dienTich, int tangId, int soPhongNgu, int soPhongTam, int loaiCanHoId)
    {
        DienTich = dienTich;
        TangId = tangId;
        SoPhongNgu = soPhongNgu;
        SoPhongTam = soPhongTam;
        LoaiCanHoId = loaiCanHoId;
    }

    public void UpdateStatus(int tinhTrangCanHoId)
    {
        TinhTrangCanHoId = tinhTrangCanHoId;
    }

    public void AddQuanHeCuTru(int userId, int loaiQuanHeCuTruId, DateTime ngayBatDau)
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
