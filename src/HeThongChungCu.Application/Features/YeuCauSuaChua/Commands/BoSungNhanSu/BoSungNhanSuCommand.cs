using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BoSungNhanSu;

public record BoSungNhanSuCommand(
    int Id,
    List<NhanSuSuaChuaRequest> NhanSu) : ICommand<YeuCauSuaChuaDetailResponse>;
