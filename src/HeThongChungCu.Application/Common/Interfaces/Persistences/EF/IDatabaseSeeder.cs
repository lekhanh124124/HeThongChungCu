namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IDatabaseSeeder
{
    Task SeedDatabaseAsync(
        int soLuongChuHo,
        int soLuongCuTru,
        int soLuongPhuongTien,
        int soLuongTaiKhoanKhach);
}
