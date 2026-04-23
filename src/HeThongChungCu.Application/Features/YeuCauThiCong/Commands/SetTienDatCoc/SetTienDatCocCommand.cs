using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.SetTienDatCoc;

public record SetTienDatCocCommand(
    int Id,
    decimal SoTien) : ICommand<YeuCauThiCongResponse>;
