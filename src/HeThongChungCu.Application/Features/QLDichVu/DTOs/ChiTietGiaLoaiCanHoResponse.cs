namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record ChiTietGiaLoaiCanHoResponse
{
    public int Id { get; init; }
    public int BangGiaId { get; init; }
    public int? LoaiCanHoId { get; init; }
    public string? LoaiCanHoTen { get; init; }
    public decimal DonGia { get; init; }
}
