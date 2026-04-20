using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.DieuPhoiNhanSu;

public record DieuPhoiNhanSuCommand(
    int Id,
    int? HopDongDoiTacId,
    List<NhanSuSuaChuaRequest> NhanSu) : ICommand<YeuCauSuaChuaDetailResponse>;
