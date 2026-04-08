using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLNhanVien.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLNhanVien.Commands.CreateNhanVien;

public record CreateNhanVienCommand(
    string Ho,
    string Ten,
    DateTime NgaySinh,
    int GioiTinhId,
    string? DiaChi,
    string? CCCD,
    string? SoDienThoai,
    string Email,
    int LoaiNhanVienId,
    DateTime NgayVaoLam,
    string? GhiChu,
    int? AnhDaiDienId,
    List<TaiLieuRequest>? TaiLieus = null) : ICommand<NhanVienDetailResponse>;
