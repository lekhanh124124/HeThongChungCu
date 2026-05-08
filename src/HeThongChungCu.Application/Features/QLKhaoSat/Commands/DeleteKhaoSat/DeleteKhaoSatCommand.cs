using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.DeleteKhaoSat;

public record DeleteKhaoSatCommand(int Id) : ICommand<bool>;
