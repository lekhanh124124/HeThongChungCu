using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.UpdateYeuCauSuaChua;

public record UpdateYeuCauSuaChuaCommand(
    int Id,
    int? PhamViId = null,
    int? LoaiSuCoId = null,
    string? NoiDung = null,
    string? MoTaViTri = null,
    List<int>? DanhSachTepIds = null,
    bool IsSubmit = false,
    bool IsWithdraw = false
) : ICommand<YeuCauSuaChuaDetailResponse>;
