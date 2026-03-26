namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record ChiTietPhiResponse(
    int DichVuId,
    string TenDichVu,
    int? CanHoId,
    double SoLuong,
    decimal DonGia,
    decimal ThanhTien,
    string GhiChu);
