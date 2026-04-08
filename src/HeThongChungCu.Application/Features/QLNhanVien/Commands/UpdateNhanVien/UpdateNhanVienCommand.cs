using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLNhanVien.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QLNhanVien.Commands.UpdateNhanVien;

public record UpdateNhanVienCommand(
    int Id,
    string Ho,
    string Ten,
    DateTime NgaySinh,
    int GioiTinhId,
    string? DiaChi,
    string? CCCD,
    string? SoDienThoai,
    int LoaiNhanVienId,
    int TrangThaiNhanVienId,
    DateTime NgayVaoLam,
    string? GhiChu,
    int? AnhDaiDienId = null,
    List<TaiLieuRequest>? TaiLieus = null) : ICommand<NhanVienDetailResponse>;
