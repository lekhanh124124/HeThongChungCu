namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record DoiTacDetailReadModel
{
    public int Id { get; init; }
    public string TenDoiTac { get; init; } = null!;
    public string? TenCongTy { get; init; }
    public string? NguoiDaiDien { get; init; }
    public string? SoGiayPhepKD { get; init; }
    public string? MaSoThue { get; init; }
    public string? DiaChi { get; init; }
    public string? SoDienThoai { get; init; }
    public string? Email { get; init; }
}
