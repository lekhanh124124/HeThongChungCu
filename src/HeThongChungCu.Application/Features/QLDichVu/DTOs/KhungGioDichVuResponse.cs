namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public class KhungGioDichVuResponse
{
    public int Id { get; set; }
    public int DichVuId { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public string TenKhungGio { get; set; } = string.Empty;
    public int? NgayTrongTuan { get; set; }
}