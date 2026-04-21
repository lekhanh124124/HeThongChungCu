using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.UpdateDichVu;

public record UpdateDichVuCommand(
    int Id,
    string TenDichVu,
    int LoaiDichVuId,
    string DonViTinh,
    string MoTa,
    int? IconId,
    bool IsBatBuoc,
    int? SoLuongToiDa) : ICommand<DichVuDetailResponse>;
