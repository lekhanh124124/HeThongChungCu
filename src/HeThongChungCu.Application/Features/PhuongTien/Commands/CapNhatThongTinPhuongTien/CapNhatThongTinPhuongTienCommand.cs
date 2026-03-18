using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.PhuongTien.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.PhuongTien.Commands.CapNhatThongTinPhuongTien;

public sealed record CapNhatThongTinPhuongTienCommand(
    int PhuongTienId,
    string TenPhuongTien,
    int LoaiPhuongTienId,
    string BienSo,
    string MauXe
) : ICommand<PhuongTienResponse>;
