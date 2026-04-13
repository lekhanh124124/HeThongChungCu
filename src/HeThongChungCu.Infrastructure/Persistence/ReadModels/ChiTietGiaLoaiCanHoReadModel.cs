namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ChiTietGiaLoaiCanHoReadModel
{
    public int Id { get; init; }
    public int? LoaiCanHoId { get; init; }
    public decimal DonGia { get; init; }
    public int BangGiaId { get; init; }
}
