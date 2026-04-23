using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.CancelYeuCauThiCong;

public record CancelYeuCauThiCongCommand(int Id, string LyDo) : ICommand<YeuCauThiCongResponse>;
