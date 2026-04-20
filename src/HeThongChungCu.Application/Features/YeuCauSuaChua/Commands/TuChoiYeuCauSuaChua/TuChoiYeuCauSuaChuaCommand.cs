using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TuChoiYeuCauSuaChua;

public record TuChoiYeuCauSuaChuaCommand(
    int Id,
    string LyDo) : ICommand<YeuCauSuaChuaDetailResponse>;
