namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class LayDSCuTruByUserIdReadModel
{
    public int Id { get; init; }
    public int LoaiQuanHeCuTruId { get; init; }

    // Thông tin căn hộ
    public int ToaNhaId { get; init; }
    public string MaToaNha { get; init; } = string.Empty;
    public string TenToaNha { get; init; } = string.Empty;
    public int TangId { get; init; }
    public string MaTang { get; init; } = string.Empty;
    public string TenTang { get; init; } = string.Empty;
    public int CanHoId { get; init; }
    public string MaCanHo { get; init; } = string.Empty;
    public string TenCanHo { get; init; } = string.Empty;

    // Thông tin quan hệ liên quan
    public int TongCuDan { get; init; } // Tổng quan hệ cư trú trong căn hộ.
}
