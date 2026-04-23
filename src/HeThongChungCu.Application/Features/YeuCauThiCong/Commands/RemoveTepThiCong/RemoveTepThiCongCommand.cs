using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.RemoveTepThiCong;

public record RemoveTepThiCongCommand(
    int Id,
    int TepId) : ICommand<YeuCauThiCongResponse>;
