using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.GiaoViecDoiTac;

public record GiaoViecDoiTacCommand(
    int Id,
    int HopDongDoiTacId,
    List<NhanSuPartnerDTO> NhanSu) : ICommand<YeuCauSuaChuaResponse>;
