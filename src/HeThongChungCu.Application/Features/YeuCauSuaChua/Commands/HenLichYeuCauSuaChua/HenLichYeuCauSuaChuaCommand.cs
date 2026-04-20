using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HenLichYeuCauSuaChua;

public record HenLichYeuCauSuaChuaCommand(
    int Id,
    DateTimeOffset HenTu,
    DateTimeOffset HenDen) : ICommand<YeuCauSuaChuaDetailResponse>;
