using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Commands.UpdateCanHo;

public record UpdateCanHoCommand(
    int Id,
    int TangId,
    string MaCanHo,
    string TenCanHo,
    decimal DienTich,
    int SoPhongNgu,
    int SoPhongTam,
    int LoaiCanHoId,
    int TinhTrangCanHoId) : ICommand<CanHoDetailResponse>;
