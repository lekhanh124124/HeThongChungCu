using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

public class YeuCauPhuongTienResponse
{
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public int? PhuongTienId { get; set; }

    public int LoaiYeuCauId { get; set; }
    public string TenLoaiYeuCau { get; set; } = string.Empty;

    public int TrangThaiId { get; set; }
    public string TenTrangThai { get; set; } = string.Empty;

    public string? NoiDung { get; set; }
    public string? LyDo { get; set; }

    public int? NguoiXuLyId { get; set; }
    public DateTimeOffset? NgayXuLy { get; set; }

    // Thông tin xe đề xuất
    public string YeuCauTenPhuongTien { get; set; } = string.Empty;
    public int YeuCauLoaiPhuongTienId { get; set; }
    public string TenYeuCauLoaiPhuongTien { get; set; } = string.Empty;
    public string YeuCauBienSo { get; set; } = string.Empty;
    public string YeuCauMauXe { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public IReadOnlyList<TepTaiLieuResponse> YeuCauHinhAnhPhuongTiens { get; set; } = new List<TepTaiLieuResponse>();
}
