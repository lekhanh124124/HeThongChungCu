using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.GiaoViecNoiBo;

public record GiaoViecNoiBoCommand(
    int Id,
    int NhanVienId) : ICommand<YeuCauSuaChuaResponse>;
