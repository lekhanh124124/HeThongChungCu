namespace HeThongChungCu.Application.Features.ToaNha.DTOs
{
    public class ToaNhaDetailResponse
    {
        public int Id { get; set; }
        public string MaToaNha { get; set; } = null!;
        public string TenToaNha { get; set; } = null!;
        public int SoCanHo { get; set; }
        public string DiaChi { get; set; } = null!;
        public string? MoTa { get; set; }
        public int TrangThaiToaNhaId { get; set; }
        public string TenTrangThaiToaNha { get; set; } = null!;
    }

}
