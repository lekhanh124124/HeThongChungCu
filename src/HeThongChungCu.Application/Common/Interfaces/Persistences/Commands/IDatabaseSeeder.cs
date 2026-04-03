using HeThongChungCu.Application.Features.Seeder.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDatabaseSeeder
{
    Task SeedDatabaseAsync(
        int soLuongChuHo,
        int soLuongCuTru,
        int soLuongPhuongTien,
        int soLuongTaiKhoanKhach,
        int soLuongNhanVien,
        YeuCauCounts? soLuongYeuCauCuTru = null,
        YeuCauCounts? soLuongYeuCauPhuongTien = null);
}
