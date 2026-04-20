using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.HoanTatXuLyYeuCauSuaChua;

public record HoanTatXuLyYeuCauSuaChuaCommand(
    int Id,
    string KetQuaXuLy,
    decimal? ChiPhiThucTe) : ICommand<YeuCauSuaChuaDetailResponse>;
