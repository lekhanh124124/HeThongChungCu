using Bogus;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class QuanHeCuTruSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int soLuongChuHo, int soLuongCuTru)
    {
        logger.LogInformation("Seeding ChuHo ({ChuHoCount}) and CuTru ({CuTruCount})...", soLuongChuHo, soLuongCuTru);

        var canHos = await context.CanHos.ToListAsync();
        if (canHos.Count == 0) return;

        var faker = new Faker("vi");

        // 1. Seed ChuHo
        for (int i = 0; i < soLuongChuHo; i++)
        {
            var canHo = faker.PickRandom(canHos);

            // Terminate existing active ChuHo if any
            var existingRelations = await context.QuanHeCuTrus
                .Where(r => r.CanHoId == canHo.Id)
                .ToListAsync();

            var activeChuHo = existingRelations
                .FirstOrDefault(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);

            if (activeChuHo != null)
            {
                activeChuHo.KetThucCuTru(DateTime.Now);
                await context.SaveChangesAsync(); // Commit termination
                // Refresh existingRelations to reflect termination
                existingRelations = await context.QuanHeCuTrus
                    .Where(r => r.CanHoId == canHo.Id)
                    .ToListAsync();
            }

            // Create new ChuHo (MUST have User + Account)
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var email = UserSeeder.GenerateEmailFromName(firstName, lastName);

            (NguoiDung user, TaiKhoan account) = await UserSeeder.CreateUserWithAccountAsync(
                context, firstName, lastName, email, Role.Resident, null!);

            var qh = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.ChuHo, DateTime.Now.AddDays(-faker.Random.Number(10, 100)));
            context.QuanHeCuTrus.Add(qh);
            await context.SaveChangesAsync();
        }

        // 2. Seed CuTru (Residents)
        // Only into apartments with an ACTIVE ChuHo
        var activeChuHoApartmentIds = await context.QuanHeCuTrus
            .Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru)
            .Select(r => r.CanHoId)
            .Distinct()
            .ToListAsync();

        if (activeChuHoApartmentIds.Count != 0)
        {
            var otherRoles = new[] { LoaiQuanHeCuTru.NguoiThue, LoaiQuanHeCuTru.NguoiOCung, LoaiQuanHeCuTru.Khac };
            var residentHasher = new HeThongChungCu.Infrastructure.Authentication.HasherService();

            for (int i = 0; i < soLuongCuTru; i++)
            {
                var canHoId = faker.PickRandom(activeChuHoApartmentIds);
                var firstName = faker.Name.FirstName();
                var lastName = faker.Name.LastName();

                var user = await UserSeeder.CreateUserOnlyAsync(context, firstName, lastName, null!);

                // Residents MUST have User, Account is optional (50% chance)
                if (faker.Random.Bool(0.5f))
                {
                    var email = UserSeeder.GenerateEmailFromName(firstName, lastName);
                    var account = new TaiKhoan(user.Id, email, email, residentHasher.HashPassword("123456"));
                    account.AddRole(Role.Resident);
                    await context.TaiKhoan.AddAsync(account);
                }

                // Get current relations for this apartment for constructor check
                var existingRelations = await context.QuanHeCuTrus
                    .Where(r => r.CanHoId == canHoId)
                    .ToListAsync();

                var qh = new QuanHeCuTru(canHoId, user.Id, faker.PickRandom(otherRoles), DateTime.Now.AddDays(-faker.Random.Number(1, 10)));
                context.QuanHeCuTrus.Add(qh);
                await context.SaveChangesAsync();
            }
        }

        logger.LogInformation("Finished seeding QuanHeCuTrus.");
    }
}
