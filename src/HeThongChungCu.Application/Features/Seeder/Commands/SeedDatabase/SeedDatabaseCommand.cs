using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.Seeder.Commands.SeedDatabase;

public record SeedDatabaseCommand(
    int SoLuongNguoiDung = 10,
    int SoLuongToaNha = 3,
    int SoLuongTangMoiToa = 10,
    int SoLuongCanHoMoiTang = 5,
    int SoLuongPhuongTien = 30,
    int SoLuongCuTru = 50,
    int SoLuongChiSoTieuThuMoiCanHo = 3,
    int SoLuongThePhuongTien = 20,
    int SoLuongTangHamMoiToa = 2) : ICommand<string>;
