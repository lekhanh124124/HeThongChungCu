using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public record TaoYeuCauCuTruCommand(
    int CanHoId,
    int LoaiYeuCauId,
    // Add Member info
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    DateTime? Dob = null,
    int? GioiTinhId = null,
    int? LoaiQuanHeId = null,
    // Action info
    int? QuanHeCuTruId = null,
    int? NewLoaiQuanHeId = null,
    string? NoiDung = null,
    List<TaiLieuRequest>? TaiLieuCuTrus = null
) : ICommand<YeuCauCuTruResponse>;
