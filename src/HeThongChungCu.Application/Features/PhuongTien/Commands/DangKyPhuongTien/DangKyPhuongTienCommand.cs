using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.PhuongTien.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.PhuongTien.Commands.DangKyPhuongTien;

public sealed record DangKyPhuongTienCommand(
    int CanHoId,
    string TenPhuongTien,
    int LoaiPhuongTienId,
    string BienSo,
    string MauXe
) : ICommand<PhuongTienResponse>;
