using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.PhuongTien.Commands.DeletePhuongTien;

public record DeletePhuongTienCommand(List<int> Ids) : ICommand<bool>;
