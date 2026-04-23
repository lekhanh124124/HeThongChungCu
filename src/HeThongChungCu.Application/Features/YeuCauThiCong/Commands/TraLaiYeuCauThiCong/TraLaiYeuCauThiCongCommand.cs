using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.TraLaiYeuCauThiCong;

public record TraLaiYeuCauThiCongCommand(int Id, string LyDo) : ICommand<YeuCauThiCongResponse>;
