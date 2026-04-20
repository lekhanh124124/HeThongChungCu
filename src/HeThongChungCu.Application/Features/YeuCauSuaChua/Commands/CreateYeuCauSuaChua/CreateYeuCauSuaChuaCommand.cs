using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;

public record CreateYeuCauSuaChuaCommand(
    int CanHoId,
    int PhamViId,
    int LoaiSuCoId,
    string? NoiDung,
    string? MoTaViTri,
    List<int>? DanhSachTepIds,
    bool IsSubmit = true
) : ICommand<YeuCauSuaChuaDetailResponse>;
