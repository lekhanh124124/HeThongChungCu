using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Tang.DTOs;
namespace HeThongChungCu.Application.Features.ToaNha.DTOs
{
    public class ToaNhaResponse
    {
        public int Id { get; set; }
        public string MaToaNha { get; set; } = null!;
        public string TenToaNha { get; set; } = null!;
        public string Block { get; set; } = null!;
        public int SoCanHo { get; set; }
        public string DiaChi { get; set; } = null!;
        public string? MoTa { get; set; }
        public int TrangThaiToaNhaId { get; set; }
        public string TenTrangThaiToaNha { get; set; } = null!;
        public IReadOnlyList<TangResponse> Tangs { get; set; } = new List<TangResponse>();
    }
}
