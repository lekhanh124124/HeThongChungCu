namespace HeThongChungCu.Application.Features.CuDan.DTOs
{
    public class ThanhVienCuTruResponse
    {
        public int QuanHeCuTruId { get; set; }
        public int UserId { get; set; }
        public int LoaiQuanHeCuTruId { get; set; }
        public string LoaiQuanHeTen { get; set; } = string.Empty;
        public DateTime NgayBatDau { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string AnhDaiDienUrl { get; set; } = string.Empty;
    }
}
