using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.CapNhatYeuCauPhuongTien;

public record CapNhatYeuCauPhuongTienCommand(
    int Id,
    int? LoaiPhuongTienId = null,
    string? TenPhuongTien = null,
    string? BienSo = null,
    string? MauXe = null,
    string? NoiDung = null,
    List<int>? FileIds = null,
    bool IsSubmit = false,
    bool IsWithdraw = false
) : ICommand<YeuCauPhuongTienResponse>;
