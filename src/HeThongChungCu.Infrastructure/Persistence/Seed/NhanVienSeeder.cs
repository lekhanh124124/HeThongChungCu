using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class NhanVienSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int count)
    {
        if (await context.NhanViens.AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding {Count} Staff Members...", count);
        var faker = new Faker("vi");
        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 0;

        var roles = new[]
        {
            LoaiNhanVien.KyThuat,
            LoaiNhanVien.VeSinh,
            LoaiNhanVien.BaoVe,
            LoaiNhanVien.QuanLy
        };

        for (int i = 0; i < count; i++)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var email = UserSeeder.GenerateEmailFromName(firstName, lastName);
            var loaiNhanVien = faker.PickRandom(roles);

            // Create User and Account with Role.Staff (No immediate SaveChanges)
            (NguoiDung user, _) = await UserSeeder.CreateUserWithAccountAsync(
                context,
                firstName,
                lastName,
                email,
                Role.Staff,
                null!,
                address: null,
                username: null,
                createdBy: adminId == 0 ? null : adminId);

            // Generate a staff code, e.g., NV-KT-0001
            var loaiCode = loaiNhanVien == LoaiNhanVien.KyThuat ? "KT" :
                          loaiNhanVien == LoaiNhanVien.VeSinh ? "VS" :
                          loaiNhanVien == LoaiNhanVien.BaoVe ? "BV" : "QL";

            var maNhanVien = $"NV-{loaiCode}-{user.Id:D4}";

            var rolesNotes = new Dictionary<LoaiNhanVien, string[]>
            {
                { LoaiNhanVien.KyThuat, ["Kỹ thuật viên điện nước, hỗ trợ sửa chữa căn hộ.", "Chuyên viên bảo trì hệ thống PCCC.", "Thợ sửa chữa điện lạnh và thiết bị tòa nhà.", "Kỹ thuật viên vận hành thang máy." ] },
                { LoaiNhanVien.VeSinh, ["Nhân viên dọn dẹp khu vực hành lang và sảnh.", "Chuyên trách thu gom rác thải cư dân.", "Nhân viên làm sạch khu vực hồ bơi và công viên.", "Vệ sinh kính mặt ngoài tòa nhà." ] },
                { LoaiNhanVien.BaoVe, ["Nhân viên trực cổng chính 24/7.", "Tuần tra khu vực hầm xe và khuôn viên.", "Trực camera an ninh tại phòng điều hành.", "Bảo vệ trực sảnh đón khách." ] },
                { LoaiNhanVien.QuanLy, ["Quản lý tòa nhà, tiếp nhận phản ánh từ cư dân.", "Kế toán ban quản lý tòa nhà.", "Trưởng bộ phận chăm sóc khách hàng.", "Giám sát vận hành dự án." ] }
            };

            var nhanVien = new NhanVien(
                user.Id,
                loaiNhanVien,
                maNhanVien,
                DateTimeOffset.Now.AddMonths(-faker.Random.Number(1, 24)),
                faker.PickRandom(rolesNotes[loaiNhanVien]));

            if (adminId != 0) nhanVien.SetCreated(adminId, DateTimeOffset.Now);

            await context.NhanViens.AddAsync(nhanVien);
        }

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();
        logger.LogInformation("Finished seeding NhanVien.");
    }
}
