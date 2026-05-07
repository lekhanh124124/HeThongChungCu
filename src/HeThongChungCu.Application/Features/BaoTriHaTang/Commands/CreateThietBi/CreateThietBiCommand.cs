using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateThietBi;

public record CreateThietBiCommand(
    string MaThietBi,
    string TenThietBi,
    string LoaiThietBi,
    string ViTri,
    DateTimeOffset NgayMua,
    DateTimeOffset? NgayHetHanBaoHanh,
    decimal? GiaTriBanDau,
    string? GhiChu) : ICommand<ThietBiResponse>;
