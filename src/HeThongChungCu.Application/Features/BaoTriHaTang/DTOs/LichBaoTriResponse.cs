namespace HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

public class LichBaoTriResponse
{
    public int Id { get; set; }
    public int ThietBiId { get; set; }
    public string TenThietBi { get; set; } = null!;
    public string MaThietBi { get; set; } = null!;
    public int HangMucBaoTriId { get; set; }
    public string TenHangMuc { get; set; } = null!;
    public string MaHangMuc { get; set; } = null!;
    public int TanSuatBaoTriId { get; set; }
    public string TenTanSuatBaoTri { get; set; } = null!;
    public DateTimeOffset NgayBatDau { get; set; }
    public DateTimeOffset? NgayKetThuc { get; set; }
    public DateTimeOffset? NgayBaoTriGanNhat { get; set; }
    public DateTimeOffset NgayBaoTriTiepTheo { get; set; }
    public bool IsActive { get; set; }
}

public class LichBaoTriDetailResponse : LichBaoTriResponse
{
    // Kế thừa toàn bộ trường thông tin chi tiết từ LichBaoTriResponse
}
