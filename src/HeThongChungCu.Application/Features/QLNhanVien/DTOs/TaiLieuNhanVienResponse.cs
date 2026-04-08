namespace HeThongChungCu.Application.Features.QLNhanVien.DTOs;

public class TaiLieuNhanVienResponse
{
    public int Id { get; set; }
    public int LoaiGiayToId { get; set; }
    public string TenLoaiGiayTo { get; set; } = null!;
    public string SoGiayTo { get; set; } = null!;
    public DateTimeOffset? NgayPhatHanh { get; set; }
    public int? TargetTaiLieuCuTruId { get; set; }
    public List<TepTaiLieuNhanVienResponse> Files { get; set; } = [];
}
