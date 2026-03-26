using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.PheDuyetYeuCauCuTru;

public record PheDuyetYeuCauCuTruCommand(
    int YeuCauCuTruId) : ICommand<YeuCauCuTruResponse>;
