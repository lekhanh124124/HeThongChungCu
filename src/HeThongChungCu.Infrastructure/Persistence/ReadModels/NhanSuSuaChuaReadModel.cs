namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class NhanSuSuaChuaReadModel
{
    public int Id { get; set; }
    public int? NhanVienId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string SoCCCD { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? VaiTro { get; set; }
    public string? GhiChu { get; set; }

    // Join fields for internal staff
    public string? StaffHo { get; set; }
    public string? StaffTen { get; set; }
    public string? StaffCCCD { get; set; }
    public string? StaffPhone { get; set; }
}
