namespace HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

public class PhieuBaoTriResponse
{
    public int Id { get; set; }
    public string MaPhieu { get; set; } = null!;
    public int ThietBiId { get; set; }
    public string TenThietBi { get; set; } = null!;
    public string MaThietBi { get; set; } = null!;
    public int HangMucBaoTriId { get; set; }
    public string TenHangMuc { get; set; } = null!;
    public int? LichBaoTriId { get; set; }
    public string? TenDoiTac { get; set; }
    public int? HopDongDoiTacId { get; set; }
    public string? SoHopDong { get; set; }
    public DateTimeOffset NgayLapPhieu { get; set; }
    public DateTimeOffset NgayDuKien { get; set; }
    public DateTimeOffset? NgayThucTe { get; set; }
    public decimal? ChiPhiThucTe { get; set; }
    public int TrangThaiPhieuBaoTriId { get; set; }
    public string TenTrangThaiPhieuBaoTri { get; set; } = null!;
    public string? GhiChuXuLy { get; set; }
    public string? LyDoHuy { get; set; }
    public int? NguoiKiemDuyetId { get; set; }
    public string? TenNguoiKiemDuyet { get; set; }
}

public class PhieuBaoTriDetailResponse : PhieuBaoTriResponse
{
    public List<PhieuBaoTriChecklistDto> Checklists { get; set; } = [];
    public List<PhieuBaoTriVatTuDto> VatTus { get; set; } = [];
    public List<NhanSuBaoTriDto> NhanSuBaoTris { get; set; } = [];
}

public class PhieuBaoTriChecklistDto
{
    public int Id { get; set; }
    public string NoiDungChecklist { get; set; } = null!;
    public bool? DatYeuCau { get; set; }
    public string? GhiChuThucTe { get; set; }
    public int? AnhMinhHoaId { get; set; }
}

public class PhieuBaoTriVatTuDto
{
    public int Id { get; set; }
    public string TenVatTu { get; set; } = null!;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}

public class NhanSuBaoTriDto
{
    public int Id { get; set; }
    public int? NhanVienId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string SoCCCD { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? VaiTro { get; set; }
}
