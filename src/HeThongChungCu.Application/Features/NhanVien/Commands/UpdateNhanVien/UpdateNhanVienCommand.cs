using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.NhanVien.Commands.UpdateNhanVien;

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
    List<TaiLieuRequest>? TaiLieus = null) : ICommand<NhanVienResponse>;
