using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.HoanCoc;

public record HoanCocCommand(
    int Id,
    decimal TienKhauTru,
    string? LyDo) : ICommand<YeuCauThiCongResponse>;
