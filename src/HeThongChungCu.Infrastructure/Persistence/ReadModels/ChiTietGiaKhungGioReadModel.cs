namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ChiTietGiaKhungGioReadModel
{
    public int Id { get; init; }
    public int KhungGioId { get; init; }
    public decimal DonGia { get; init; }
    public string TenKhungGio { get; init; } = string.Empty;
    public int BangGiaId { get; init; }
}
