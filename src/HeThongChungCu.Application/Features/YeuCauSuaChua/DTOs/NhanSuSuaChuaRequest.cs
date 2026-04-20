namespace HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

public record NhanSuSuaChuaRequest(
    int? NhanVienId,
    string? HoTen,
    string? SoCCCD,
    string? SoDienThoai,
    string? VaiTro,
    string? GhiChu);
