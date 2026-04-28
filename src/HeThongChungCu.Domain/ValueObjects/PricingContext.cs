using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.ValueObjects;

/// <summary>
/// Context chứa các tham số đầu vào để tính phí dịch vụ.
/// </summary>
public record PricingContext(
    decimal SoLuong = 1,
    decimal? ChiSoDau = null,
    decimal? ChiSoCuoi = null,
    decimal? DienTich = null,
    LoaiCanHo? LoaiCanHoId = null,
    int? KhungGioId = null,
    decimal? SoTienGoc = null,
    int? SoNgayQuaHan = null
);
