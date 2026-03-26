namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record DichVuResponse(
    int Id,
    string MaDichVu,
    string TenDichVu,
    string DonViTinh,
    bool IsActive);
