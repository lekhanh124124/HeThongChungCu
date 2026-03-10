namespace HeThongChungCu.Application.Features.CanHo.DTOs
{
    public class QuanHeCuTruDetailResponse
    {
        public int Id { get; set; }
        public int CanHoId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int LoaiQuanHeCuTruId { get; set; }
        public string TenLoaiQuanHeCuTru { get; set; } = string.Empty;
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public bool IsKetThuc { get; set; }
    }
}
