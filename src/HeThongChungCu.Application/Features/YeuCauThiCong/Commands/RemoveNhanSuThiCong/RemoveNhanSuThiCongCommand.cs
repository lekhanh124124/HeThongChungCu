using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.RemoveNhanSuThiCong;

public record RemoveNhanSuThiCongCommand(
    int Id,
    int NhanSuId,
    string LyDo) : ICommand<YeuCauThiCongResponse>;
