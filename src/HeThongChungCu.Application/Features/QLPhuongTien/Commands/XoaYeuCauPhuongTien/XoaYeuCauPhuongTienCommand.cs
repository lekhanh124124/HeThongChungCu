namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.XoaYeuCauPhuongTien;

public record XoaYeuCauPhuongTienCommand(List<int> Ids) : ICommand<bool>;
