namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record ChiTietGiaLuyTienResponse
{
    public int Id { get; init; }
    public int BangGiaId { get; init; }
    public decimal TuMuc { get; init; }
    public decimal? DenMuc { get; init; }
    public decimal DonGia { get; init; }
}
