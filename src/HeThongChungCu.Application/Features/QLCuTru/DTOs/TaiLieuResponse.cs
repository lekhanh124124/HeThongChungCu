namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public class TaiLieuResponse
{
    public int Id { get; set; }
    public int LoaiGiayToId { get; set; }
    public string TenLoaiGiayTo { get; set; } = null!;
    public string SoGiayTo { get; set; } = null!;
    public DateTime? NgayPhatHanh { get; set; }
    public List<TepTaiLieuResponse> Files { get; set; } = [];
}
