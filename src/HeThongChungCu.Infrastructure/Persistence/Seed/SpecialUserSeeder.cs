using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class SpecialUserSeeder
{
    private static readonly HasherService _passwordHasher = new();

    public static async Task SeedGiangKietAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding Special User: Giang Tuấn Kiệt...");

        var canHos = await context.CanHos.ToListAsync();
        if (canHos.Count < 4)
        {
            logger.LogWarning("Not enough apartments to seed special user case.");
            return;
        }

        var faker = new Faker("vi");

        // 1. Create Special User and Account
        var user = new NguoiDung(
            "Tuấn Kiệt",
            "Giang",
            new DateTime(2004, 1, 1),
            GioiTinh.Nam,
            "TP. Hồ Chí Minh",
            UserSeeder.RegisterIdCard("001004123456"),
            UserSeeder.RegisterPhoneNumber("0912345678"));

        await context.NguoiDung.AddAsync(user);
        await context.SaveChangesAsync();

        var email = UserSeeder.RegisterEmail("giangkiet2k4@gmail.com");
        var account = new TaiKhoan(user.Id, email, email, _passwordHasher.HashPassword("123456."));
        account.AddRole(Role.Resident);
        await context.TaiKhoan.AddAsync(account);
        await context.SaveChangesAsync();

        // 2. Pick 4 apartments
        var selectedCanHos = canHos.OrderBy(x => Guid.NewGuid()).Take(4).ToList();

        for (int i = 0; i < selectedCanHos.Count; i++)
        {
            var canHo = selectedCanHos[i];
            
            // Terminate existing active ChuHo if any
            var existingRelations = await context.QuanHeCuTrus
                .Where(r => r.CanHoId == canHo.Id)
                .ToListAsync();

            var activeChuHos = existingRelations
                .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru)
                .ToList();

            foreach (var ch in activeChuHos)
            {
                ch.KetThucCuTru(DateTime.Now);
            }
            await context.SaveChangesAsync();

            // Refresh relations for constructor
            existingRelations = await context.QuanHeCuTrus.Where(r => r.CanHoId == canHo.Id).ToListAsync();

            // 3 active, 1 terminated
            var isTerminated = i == 3;
            var qh = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.ChuHo, DateTime.Now.AddMonths(-6), existingRelations);
            
            if (isTerminated)
            {
                qh.KetThucCuTru(DateTime.Now.AddMonths(-1));
            }

            context.QuanHeCuTrus.Add(qh);
            await context.SaveChangesAsync();

            // 3. Add 3-5 residents per apartment
            int residentCount = faker.Random.Number(3, 5);
            var otherRoles = new[] { LoaiQuanHeCuTru.NguoiOCung, LoaiQuanHeCuTru.NguoiThue };
            
            for (int j = 0; j < residentCount; j++)
            {
                var rFirstName = faker.Name.FirstName();
                var rLastName = faker.Name.LastName();
                var rUser = await UserSeeder.CreateUserOnlyAsync(context, rFirstName, rLastName, faker.Phone.PhoneNumber("09########"));
                
                var currentRelations = await context.QuanHeCuTrus.Where(r => r.CanHoId == canHo.Id).ToListAsync();
                var rqh = new QuanHeCuTru(canHo.Id, rUser.Id, faker.PickRandom(otherRoles), DateTime.Now.AddMonths(-5), currentRelations);
                
                context.QuanHeCuTrus.Add(rqh);
            }
            await context.SaveChangesAsync();

            // 4. Add 2 vehicles and 4 cards total per apartment
            for (int k = 0; k < 2; k++)
            {
                var loai = k == 0 ? LoaiPhuongTien.Oto : LoaiPhuongTien.XeMay;
                var model = k == 0 ? "Toyota Camry" : "Honda SH";
                var bienSo = PhuongTienSeeder.RegisterBienSo($"{faker.Random.Int(29, 31)}{faker.Random.String2(1, "ABCDEFGHJK")}-{faker.Random.Int(10000, 99999)}");

                var pt = new PhuongTien(canHo.Id, model, loai, bienSo, model);
                await context.PhuongTiens.AddAsync(pt);
                await context.SaveChangesAsync();

                pt.UpdateTrangThai(TrangThaiPhuongTien.Approved, DateTime.Now);
                
                // 2 cards per vehicle (Total 4 per apartment)
                for (int c = 0; c < 2; c++)
                {
                    var maThe = PhuongTienSeeder.RegisterMaThe(faker.Random.Replace("GK-##########"));
                    var the = pt.AddThe(maThe, DateTime.Now.AddMonths(-4));

                    // If we add multiple cards, the older ones must be locked (Business Rule)
                    if (c == 0)
                    {
                        the.KhoaThe(DateTime.Now.AddMonths(-3));
                    }
                }
                await context.SaveChangesAsync();
            }
        }

        logger.LogInformation("Finished seeding Special User: Giang Tuấn Kiệt.");
    }
}
