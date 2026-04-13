namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ChiTietGiaLuyTienReadModel
{
    public int Id { get; init; }
    public decimal TuMuc { get; init; }
    public decimal DenMuc { get; init; }
    public decimal DonGia { get; init; }
    public int BangGiaId { get; init; }
}
