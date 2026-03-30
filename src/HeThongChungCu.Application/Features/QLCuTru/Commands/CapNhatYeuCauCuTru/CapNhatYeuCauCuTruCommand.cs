using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatYeuCauCuTru;

public record CapNhatYeuCauCuTruCommand(
    int Id,
    // Add Member info
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    DateTime? Dob = null,
    int? GioiTinhId = null,
    int? LoaiQuanHeId = null,
    // Action info
    int? NewLoaiQuanHeId = null,
    string? NoiDung = null,
    List<TaiLieuRequest>? TaiLieuCuTrus = null,
    bool IsSubmit = false,
    bool IsWithdraw = false
) : ICommand<YeuCauCuTruResponse>;
