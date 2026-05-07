using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateThietBi;

public record UpdateThietBiCommand(
    int Id,
    string TenThietBi,
    string LoaiThietBi,
    string ViTri,
    DateTimeOffset NgayMua,
    DateTimeOffset? NgayHetHanBaoHanh,
    decimal? GiaTriBanDau,
    int? TrangThaiThietBiId,
    string? GhiChu) : ICommand<ThietBiResponse>;
