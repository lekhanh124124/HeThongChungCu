namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record ChiTietGiaKhungGioResponse
{
    public int Id { get; init; }
    public int BangGiaId { get; init; }
    public int KhungGioId { get; init; }
    public string TenKhungGio { get; init; } = string.Empty;
    public decimal DonGia { get; init; }
}