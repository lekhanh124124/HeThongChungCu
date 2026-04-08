namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record BangGiaResponse
{
    public int Id { get; init; }
    public int DichVuId { get; init; }
    public string TenBangGia { get; init; } = string.Empty;
    public DateTime NgayApDung { get; init; }
    public DateTime? NgayKetThuc { get; init; }
    public int LoaiDinhGiaId { get; init; }
    public string LoaiDinhGiaTen { get; init; } = string.Empty;
    public decimal? DonGia { get; init; }
    public bool IsActive { get; init; }
    public List<ChiTietGiaLuyTienResponse> GiaLuyTiens { get; init; } = [];
    public List<ChiTietGiaKhungGioResponse> GiaKhungGios { get; init; } = [];
    public List<ChiTietGiaLoaiCanHoResponse> GiaLoaiCanHos { get; init; } = [];
}