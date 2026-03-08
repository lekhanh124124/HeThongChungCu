using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.CreateCanHo;

public record CreateCanHoCommand(
    int ToaNhaId,
    string MaCanHo,
    decimal DienTich,
    int Tang,
    int SoPhongNgu,
    int SoPhongTam,
    int TinhTrangCanHoId) : ICommand<CanHoResponse>;
