namespace HeThongChungCu.Application.Features.QLDoiTac.DTOs;

public class HoaDonDoiTacDetailResponse : HoaDonDoiTacResponse
{
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Thêm các thông tin đối tác mở rộng để kế toán xem nhanh
    public string? TenCongTy { get; set; }
    public string? NguoiDaiDienDoiTac { get; set; }
    public string? SoDienThoaiDoiTac { get; set; }
    public string? EmailDoiTac { get; set; }
    public string? NoiDungHopDong { get; set; }
}
