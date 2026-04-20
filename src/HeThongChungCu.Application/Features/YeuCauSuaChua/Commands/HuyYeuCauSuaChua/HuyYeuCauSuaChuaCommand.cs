using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HuyYeuCauSuaChua;

public record HuyYeuCauSuaChuaCommand(
    int Id,
    string LyDoHuy) : ICommand<YeuCauSuaChuaDetailResponse>;
