namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class LayThongTinCuDanReadModel
{
    public int NguoiDungId { get; init; }
    public string HoTen { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateTime NgaySinh { get; init; }
    public int GioiTinhId { get; init; }
    public int RoleId { get; init; }
    public string AnhDaiDienUrl { get; init; } = string.Empty;

    public int QuanHeCuTruId { get; init; }
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTime NgayBatDau { get; init; }
}
