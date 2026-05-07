using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ConfirmChiSoBatch;

public record ConfirmChiSoBatchCommand(
    int Thang,
    int Nam,
    int? DichVuId = null) : ICommand<int>;
