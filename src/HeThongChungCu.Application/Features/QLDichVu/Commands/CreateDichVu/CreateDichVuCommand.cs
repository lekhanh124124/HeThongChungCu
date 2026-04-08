using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateDichVu;

public record CreateDichVuCommand(
    string MaDichVu,
    string TenDichVu,
    int LoaiDichVuId,
    string DonViTinh,
    string? MoTa = null,
    int? IconId = null,
    int? HopDongDoiTacId = null,
    bool IsBatBuoc = false,
    int? SoLuongToiDa = null) : ICommand<DichVuResponse>;
