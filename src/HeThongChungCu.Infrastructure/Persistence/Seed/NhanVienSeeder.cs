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
                null!);

            // Generate a staff code, e.g., NV-KT-0001
            var loaiCode = loaiNhanVien == LoaiNhanVien.KyThuat ? "KT" :
                          loaiNhanVien == LoaiNhanVien.VeSinh ? "VS" :
                          loaiNhanVien == LoaiNhanVien.BaoVe ? "BV" : "QL";
            
            var maNhanVien = $"NV-{loaiCode}-{user.Id:D4}";
            
            var nhanVien = new NhanVien(
                user.Id, 
                loaiNhanVien, 
                maNhanVien, 
                DateTimeOffset.UtcNow.AddMonths(-faker.Random.Number(1, 24)),
                faker.Lorem.Sentence());

            await context.NhanViens.AddAsync(nhanVien);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Finished seeding NhanVien.");
    }
}
