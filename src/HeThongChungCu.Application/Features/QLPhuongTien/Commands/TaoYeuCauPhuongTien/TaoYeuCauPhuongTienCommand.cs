using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;

public record TaoYeuCauPhuongTienCommand(
    int CanHoId,
    int? PhuongTienId,
    int LoaiYeuCauId,
    int? LoaiPhuongTienId,
    string? TenPhuongTien,
    string? BienSo,
    string? MauXe,
    string? NoiDung,
    List<int>? FileIds
) : ICommand<YeuCauPhuongTienResponse>;
