using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public record BangGiaResponse(
    int Id,
    int DichVuId,
    string TenBangGia,
    DateTime NgayApDung,
    DateTime? NgayKetThuc,
    decimal DonGia,
    int LoaiDinhGiaId,
    bool IsActive,
    List<BangGiaLuyTienResponse> LuyTiens);

public record BangGiaLuyTienResponse(
    double TuMuc,
    double? DenMuc,
    decimal DonGia);
