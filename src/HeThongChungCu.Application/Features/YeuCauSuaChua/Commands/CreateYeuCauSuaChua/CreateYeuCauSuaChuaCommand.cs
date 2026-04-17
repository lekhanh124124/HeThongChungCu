using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;

public record CreateYeuCauSuaChuaCommand(
    int CanHoId,
    int PhamViId,
    int LoaiSuCoId,
    int MucDoUuTienDeXuatId,
    string? NoiDung,
    string? MoTaViTri,
    List<int>? DanhSachTepIds
) : ICommand<YeuCauSuaChuaResponse>;
