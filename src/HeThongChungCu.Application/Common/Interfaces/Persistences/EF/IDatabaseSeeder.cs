namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IDatabaseSeeder
{
    Task SeedDatabaseAsync(
        int soLuongNguoiDung, 
        int soLuongToaNha, 
        int soLuongTangMoiToa, 
        int soLuongCanHoMoiTang, 
        int soLuongPhuongTien,
        int soLuongCuTru,
        int soLuongChiSoTieuThuMoiCanHo,
        int soLuongThePhuongTien,
        int soLuongTangHamMoiToa);
}
