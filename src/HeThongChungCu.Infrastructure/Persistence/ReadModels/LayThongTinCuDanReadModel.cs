namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class LayThongTinCuDanReadModel
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string IdCard { get; init; } = string.Empty;
    public DateTime Dob { get; init; }
    public int GioiTinhId { get; init; }
    public int RoleId { get; init; }
    public string AnhDaiDienUrl { get; init; } = string.Empty;

    public int QuanHeCuTruId { get; init; }
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTime NgayBatDau { get; init; }
}
