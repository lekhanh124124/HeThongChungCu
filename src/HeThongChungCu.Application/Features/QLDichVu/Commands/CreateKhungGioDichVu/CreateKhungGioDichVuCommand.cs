using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateKhungGioDichVu;

public record CreateKhungGioDichVuCommand(
    int DichVuId,
    TimeSpan GioBatDau,
    TimeSpan GioKetThuc,
    string TenKhungGio,
    int? NgayTrongTuan = null) : ICommand<KhungGioDichVuResponse>;
