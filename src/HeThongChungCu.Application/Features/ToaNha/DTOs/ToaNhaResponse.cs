using HeThongChungCu.Application.Features.CanHo.DTOs;
namespace HeThongChungCu.Application.Features.ToaNha.DTOs
{
    public class ToaNhaResponse
    {
        public int Id { get; set; }
        public string MaToaNha { get; set; } = null!;
        public string TenToaNha { get; set; } = null!;
        public int SoCanHo { get; set; }
        public string DiaChi { get; set; } = null!;
        public string? MoTa { get; set; }
        public int TrangThaiToaNhaId { get; set; }
        public string TenTrangThaiToaNha { get; set; } = null!;
        public IReadOnlyList<CanHoDetailResponse> CanHos { get; set; } = new List<CanHoDetailResponse>();
    }
}
