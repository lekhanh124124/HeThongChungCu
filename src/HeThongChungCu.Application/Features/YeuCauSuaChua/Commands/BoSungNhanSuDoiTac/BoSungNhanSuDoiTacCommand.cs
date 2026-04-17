using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BoSungNhanSuDoiTac;

public record BoSungNhanSuDoiTacCommand(
    int Id,
    List<NhanSuPartnerDTO> NhanSu) : ICommand<YeuCauSuaChuaResponse>;
