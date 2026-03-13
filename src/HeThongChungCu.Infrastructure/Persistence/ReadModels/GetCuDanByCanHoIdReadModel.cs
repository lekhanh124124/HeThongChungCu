namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class GetCuDanByCanHoIdReadModel
{
    public int QuanHeCuTruId { get; init; }
    public int UserId { get; init; }
    public string HoTen { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTime NgayBatDau { get; init; }
}
