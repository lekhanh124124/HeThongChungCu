using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateHangMucBaoTri;

public record UpdateHangMucBaoTriCommand(
    int Id,
    string TenHangMuc,
    string? MoTa,
    int ThoiGianUocTinhPhut,
    decimal ChiPhiUocTinh,
    List<string> ChecklistTieuChuan) : ICommand<HangMucBaoTriDetailResponse>;
