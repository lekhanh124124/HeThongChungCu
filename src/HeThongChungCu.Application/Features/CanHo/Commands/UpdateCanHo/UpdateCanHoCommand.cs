using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public record UpdateCanHoCommand(
    int Id,
    decimal DienTich,
    int TangId,
    int SoPhongNgu,
    int SoPhongTam,
    int LoaiCanHoId,
    int TinhTrangCanHoId) : ICommand<CanHoDetailResponse>;
