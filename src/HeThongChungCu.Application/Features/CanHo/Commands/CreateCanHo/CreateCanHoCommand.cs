using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Commands.CreateCanHo;

public record CreateCanHoCommand(
    string MaCanHo,
    string TenCanHo,
    decimal DienTich,
    int TangId,
    int SoPhongNgu,
    int SoPhongTam,
    int LoaiCanHoId) : ICommand<CanHoDetailResponse>;
