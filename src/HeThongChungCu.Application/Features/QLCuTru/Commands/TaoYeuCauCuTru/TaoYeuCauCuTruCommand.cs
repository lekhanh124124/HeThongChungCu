using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public record TaoYeuCauCuTruCommand(
    int CanHoId,
    int? TargetQuanHeCuTruId,
    int LoaiYeuCauId,
    string? NoiDung,
    // Add Member info
    string? FirstName,
    string? LastName,
    int? GioiTinhId,
    DateTime? Dob,
    string? CCCD,
    string? PhoneNumber,
    string? DiaChi,
    int? LoaiQuanHeId,
    // Action info
    List<TaiLieuRequest>? TaiLieuCuTrus,
    bool IsSubmit
) : ICommand<YeuCauCuTruResponse>;
