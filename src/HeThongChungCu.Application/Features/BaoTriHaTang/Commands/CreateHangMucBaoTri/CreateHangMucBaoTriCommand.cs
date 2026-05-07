using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateHangMucBaoTri;

public record CreateHangMucBaoTriCommand(
    string MaHangMuc,
    string TenHangMuc,
    string? MoTa,
    int ThoiGianUocTinhPhut,
    decimal ChiPhiUocTinh,
    List<string> ChecklistTieuChuan) : ICommand<HangMucBaoTriDetailResponse>;
