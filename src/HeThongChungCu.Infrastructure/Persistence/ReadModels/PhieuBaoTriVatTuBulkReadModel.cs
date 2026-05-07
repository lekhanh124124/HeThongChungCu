namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record PhieuBaoTriVatTuBulkReadModel
{
    public int Id { get; init; }
    public int PhieuBaoTriId { get; init; }
    public string TenVatTu { get; init; } = string.Empty;
    public int SoLuong { get; init; }
    public decimal DonGia { get; init; }
    public decimal ThanhTien { get; init; }
}
