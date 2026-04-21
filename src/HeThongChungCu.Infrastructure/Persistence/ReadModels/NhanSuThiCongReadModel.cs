namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class NhanSuThiCongReadModel
{
    public int Id { get; set; }
    public int? NhanVienId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string SoCCCD { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? VaiTro { get; set; }
    public string? GhiChu { get; set; }
    public string? LyDoXoa { get; set; }

    // Personnel from Employee (NhanVien) joining
    public string? StaffHo { get; set; }
    public string? StaffTen { get; set; }
    public string? StaffCCCD { get; set; }
    public string? StaffPhone { get; set; }
}
