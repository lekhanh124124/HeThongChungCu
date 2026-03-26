namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record ChiSoTieuThuResponse(
    int Id,
    int CanHoId,
    int DichVuId,
    double ChiSoCu,
    double ChiSoMoi,
    double SoLuong,
    int Thang,
    int Nam,
    DateTime NgayChot,
    bool IsLock);
