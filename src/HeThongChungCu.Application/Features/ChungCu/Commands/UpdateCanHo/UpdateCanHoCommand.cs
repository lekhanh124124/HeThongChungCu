using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.UpdateCanHo;

public record UpdateCanHoCommand(
    int Id,
    decimal DienTich,
    int Tang,
    int SoPhongNgu,
    int SoPhongTam,
    int TinhTrangCanHoId) : ICommand<CanHoResponse>;
