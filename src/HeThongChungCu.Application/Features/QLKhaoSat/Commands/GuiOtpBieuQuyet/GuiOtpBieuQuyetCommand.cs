using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.GuiOtpBieuQuyet;

public record GuiOtpBieuQuyetCommand : ICommand<string>
{
    public int KhaoSatId { get; init; }
    public int CanHoId { get; init; }
    public int NguoiDungId { get; init; }
}
