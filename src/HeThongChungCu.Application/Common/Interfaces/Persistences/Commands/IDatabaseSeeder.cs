namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDatabaseSeeder
{
    Task SeedDatabaseAsync(
        int soLuongChuHo,
        int soLuongCuTru,
        int soLuongPhuongTien,
        int soLuongTaiKhoanKhach);
}
