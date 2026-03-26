using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.Seeder.Commands.SeedDatabase;

public record SeedDatabaseCommand(
    int SoLuongChuHo = 10,
    int SoLuongCuTru = 30,
    int SoLuongPhuongTien = 20,
    int SoLuongTaiKhoanKhach = 5) : ICommand<string>;
