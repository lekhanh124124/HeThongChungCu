using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.PheDuyetYeuCauPhuongTien;

public record PheDuyetYeuCauPhuongTienCommand(int YeuCauPhuongTienId) : ICommand<YeuCauPhuongTienResponse>;
