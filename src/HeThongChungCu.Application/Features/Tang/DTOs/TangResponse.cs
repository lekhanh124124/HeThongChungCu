using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.Tang.DTOs;

public class TangResponse
{
    public int Id { get; set; }
    public string MaTang { get; set; } = null!;
    public string TenTang { get; set; } = null!;
    public int LoaiTangId { get; set; }
    public string TenLoaiTang { get; set; } = null!;
    public int ToaNhaId { get; set; }
    public string TenToaNha { get; set; } = null!;
    public IReadOnlyList<CanHoDetailResponse> CanHos { get; set; } = new List<CanHoDetailResponse>();
}
