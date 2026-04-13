namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record KhungGioDichVuReadModel
{
    public int? TotalCount { get; init; }
    public int Id { get; init; }
    public int DichVuId { get; init; }
    public TimeSpan GioBatDau { get; init; }
    public TimeSpan GioKetThuc { get; init; }
    public string TenKhungGio { get; init; } = string.Empty;
    public int? NgayTrongTuan { get; init; }
    public bool IsActive { get; init; }
}
