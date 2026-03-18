namespace HeThongChungCu.Application.Features.PhuongTien.Commands.KhoaThePhuongTien;

public record KhoaThePhuongTienCommand(List<int> TheIds) : ICommand<bool>;

