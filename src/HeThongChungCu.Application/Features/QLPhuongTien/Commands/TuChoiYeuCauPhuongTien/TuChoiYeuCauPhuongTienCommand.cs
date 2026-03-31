using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.TuChoiYeuCauPhuongTien;

public record TuChoiYeuCauPhuongTienCommand(
    int YeuCauPhuongTienId,
    string LyDo) : ICommand<YeuCauPhuongTienResponse>;
