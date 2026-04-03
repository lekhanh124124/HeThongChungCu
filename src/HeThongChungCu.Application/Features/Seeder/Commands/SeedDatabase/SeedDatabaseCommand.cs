using HeThongChungCu.Application.Features.Seeder.DTOs;

namespace HeThongChungCu.Application.Features.Seeder.Commands.SeedDatabase;

public record SeedDatabaseCommand(
    int SoLuongChuHo = 50,
    int SoLuongCuTru = 200,
    int SoLuongPhuongTien = 150,
    int SoLuongTaiKhoanKhach = 50,
    int SoLuongNhanVien = 20,
    YeuCauCounts? SoLuongYeuCauCuTru = null,
    YeuCauCounts? SoLuongYeuCauPhuongTien = null) : ICommand<string>;
