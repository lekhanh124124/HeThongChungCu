using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.ActivateKhungGioDichVu;

public record ActivateKhungGioDichVuCommand(int DichVuId, List<int> Ids) : ICommand<bool>;
