using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.BaoMatThePhuongTien;

public record BaoMatThePhuongTienCommand(List<int> TheIds) : ICommand<bool>;
