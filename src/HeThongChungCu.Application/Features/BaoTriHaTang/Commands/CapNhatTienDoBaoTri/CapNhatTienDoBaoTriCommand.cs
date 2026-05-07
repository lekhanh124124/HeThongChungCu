using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CapNhatTienDoBaoTri;

public record CapNhatTienDoBaoTriCommand(
    int Id,
    string? GhiChuXuLy,
    List<UpdateChecklistInput> Checklists,
    List<VatTuInput>? VatTus) : ICommand<PhieuBaoTriDetailResponse>;

public record UpdateChecklistInput(
    int ChecklistId,
    bool IsDatYeuCau,
    string? GhiChuThucTe,
    int? AnhMinhHoaId
);

public record VatTuInput(
    string TenVatTu,
    int SoLuong,
    decimal DonGia
);
