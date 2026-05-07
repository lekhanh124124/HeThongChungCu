namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record NhanSuBaoTriBulkReadModel
{
    public int Id { get; init; }
    public int PhieuBaoTriId { get; init; }
    public int? NhanVienId { get; init; }
    public string HoTen { get; init; } = string.Empty;
    public string SoCCCD { get; init; } = string.Empty;
    public string? SoDienThoai { get; init; }
    public string? VaiTro { get; init; }
}
