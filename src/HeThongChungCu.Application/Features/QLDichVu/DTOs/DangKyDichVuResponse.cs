namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record DangKyDichVuResponse(
    int Id,
    int CanHoId,
    int DichVuId,
    DateTime NgayBatDau,
    DateTime? NgayKetThuc,
    bool IsActive);
