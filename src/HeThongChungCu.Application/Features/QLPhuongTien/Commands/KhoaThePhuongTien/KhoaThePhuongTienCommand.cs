namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KhoaThePhuongTien;

public record KhoaThePhuongTienCommand(List<int> TheIds) : ICommand<bool>;

