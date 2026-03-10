using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Commands.CreateCanHo;

public record CreateCanHoCommand(
    int ToaNhaId,
    string MaCanHo,
    decimal DienTich,
    int Tang,
    int SoPhongNgu,
    int SoPhongTam,
    int LoaiCanHoId) : ICommand<CanHoDetailResponse>;
