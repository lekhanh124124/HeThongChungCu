using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.XoaNhanSuSuaChua;

public record XoaNhanSuSuaChuaCommand(
    int Id,
    int NhanSuId,
    string LyDo) : ICommand<YeuCauSuaChuaDetailResponse>;
