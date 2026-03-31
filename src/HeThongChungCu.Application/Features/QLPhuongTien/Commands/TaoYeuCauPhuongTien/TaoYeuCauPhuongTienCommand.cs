using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TaoYeuCauPhuongTien;

public record TaoYeuCauPhuongTienCommand(
    int CanHoId,
    int? YeuCauPhuongTienId,
    int LoaiYeuCauId,
    int? YeuCauLoaiPhuongTienId,
    string? YeuCauTenPhuongTien,
    string? YeuCauBienSo,
    string? YeuCauMauXe,
    string? NoiDung,
    List<int>? FileIds
) : ICommand<YeuCauPhuongTienResponse>;
