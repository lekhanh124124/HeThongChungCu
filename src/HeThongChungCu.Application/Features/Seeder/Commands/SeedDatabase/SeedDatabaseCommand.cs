using HeThongChungCu.Application.Features.Seeder.DTOs;

namespace HeThongChungCu.Application.Features.Seeder.Commands.SeedDatabase;

public record SeedDatabaseCommand(
    int SoLuongChuHo = 50,
    int SoLuongCuTru = 300,
    int SoLuongPhuongTien = 200,
    int SoLuongTaiKhoanKhach = 50,
    int SoLuongNhanVien = 50,
    int SoLuongYeuCauSuaChua = 50,
    int SoLuongYeuCauThiCong = 50,
    YeuCauCounts? SoLuongYeuCauCuTru = null,
    YeuCauCounts? SoLuongYeuCauPhuongTien = null) : ICommand<string>;
