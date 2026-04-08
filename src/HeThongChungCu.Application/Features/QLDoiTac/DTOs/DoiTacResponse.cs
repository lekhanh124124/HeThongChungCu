using HeThongChungCu.Application.Features.UploadMedia.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.DTOs;

public class DoiTacResponse
{
    public int Id { get; set; }
    public string TenDoiTac { get; set; } = string.Empty;
    public string? TenCongTy { get; set; }
    public string? NguoiDaiDien { get; set; }
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
    public string? GhiChu { get; set; }
    public DateTimeOffset? NgayHetHan { get; set; }
}

