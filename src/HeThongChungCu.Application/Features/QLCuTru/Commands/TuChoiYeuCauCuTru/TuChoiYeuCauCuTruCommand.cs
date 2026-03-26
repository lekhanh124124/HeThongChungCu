using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TuChoiYeuCauCuTru;

public record TuChoiYeuCauCuTruCommand(
    int YeuCauCuTruId,
    string LyDo) : ICommand<YeuCauCuTruResponse>;
