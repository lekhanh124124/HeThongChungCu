using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.XacNhanThuCoc;

public record XacNhanThuCocCommand(
    int Id,
    string? GhiChu) : ICommand<YeuCauThiCongResponse>;
