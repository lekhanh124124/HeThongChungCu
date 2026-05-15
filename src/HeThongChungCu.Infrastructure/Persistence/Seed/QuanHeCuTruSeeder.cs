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

        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 0;

        var canHos = await context.CanHos.ToListAsync();
        if (canHos.Count == 0) return;

        // Pre-fetch all active relations to avoid N+1 and navigation property issues
        var allRelations = await context.QuanHeCuTrus
            .Where(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru)
            .ToListAsync();

        var relationsByCanHo = allRelations.GroupBy(r => r.CanHoId).ToDictionary(g => g.Key, g => g.ToList());

        var faker = new Faker("vi");
        var availableCanHos = canHos.OrderBy(_ => Guid.NewGuid()).ToList();

        // 1. Seed Owners (ChuHo)
        int chuHoCount = Math.Min(soLuongChuHo, availableCanHos.Count / 2); // Seed owners for up to half of apartments
        var chuHoCanHos = availableCanHos.Take(chuHoCount).ToList();
        var headApartmentDates = new Dictionary<int, DateTimeOffset>();

        foreach (var canHo in chuHoCanHos)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var email = UserSeeder.GenerateEmailFromName(firstName, lastName);

            (NguoiDung user, _) = await UserSeeder.CreateUserWithAccountAsync(
                context, firstName, lastName, email, Role.Resident, null!, null, null, adminId == 0 ? null : adminId);

            var joinDate = DateTimeOffset.Now.AddDays(-faker.Random.Number(30, 120));
            var qh = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.ChuHo, joinDate);
            if (adminId != 0) qh.SetCreated(adminId, joinDate);
            context.QuanHeCuTrus.Add(qh);

            canHo.SyncStatusWithResidency(hasOwner: true, hasTenant: false);
            headApartmentDates[canHo.Id] = joinDate;
        }

        // 2. Seed Tenants (NguoiThue) for remaining vacant apartments
        var remainingCanHos = availableCanHos.Skip(chuHoCount).ToList();
        int tenantCount = faker.Random.Number(10, Math.Min(20, remainingCanHos.Count));
        var tenantCanHos = remainingCanHos.Take(tenantCount).ToList();

        foreach (var canHo in tenantCanHos)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var email = UserSeeder.GenerateEmailFromName(firstName, lastName);

            (NguoiDung user, _) = await UserSeeder.CreateUserWithAccountAsync(
                context, firstName, lastName, email, Role.Resident, null!, null, null, adminId == 0 ? null : adminId);

            var joinDate = DateTimeOffset.Now.AddDays(-faker.Random.Number(10, 60));
            var qh = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.NguoiThue, joinDate);
            if (adminId != 0) qh.SetCreated(adminId, joinDate);
            context.QuanHeCuTrus.Add(qh);

            canHo.SyncStatusWithResidency(hasOwner: false, hasTenant: true);
            headApartmentDates[canHo.Id] = joinDate;
        }

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        // 3. Seed Others (NguoiOCung, Khac) for apartments that already have a head
        if (headApartmentDates.Count != 0)
        {
            var headApartmentIds = headApartmentDates.Keys.ToList();
            var otherRoles = new[] { LoaiQuanHeCuTru.NguoiOCung, LoaiQuanHeCuTru.Khac };
            int batchSize = 50;
            for (int i = 0; i < soLuongCuTru; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, soLuongCuTru - i);
                var batchUsers = new List<(NguoiDung User, int CanHoId)>();

                for (int j = 0; j < currentBatchSize; j++)
                {
                    var canHoId = faker.PickRandom(headApartmentIds);
                    var firstName = faker.Name.FirstName();
                    var lastName = faker.Name.LastName();

                    if (faker.Random.Bool(0.5f))
                    {
                        var email = UserSeeder.GenerateEmailFromName(firstName, lastName);
                        var userAndAccount = await UserSeeder.CreateUserWithAccountAsync(context, firstName, lastName, email, Role.Resident, null!, null, null, adminId == 0 ? null : adminId);
                        batchUsers.Add((userAndAccount.NguoiDung, canHoId));
                    }
                    else
                    {
                        var user = await UserSeeder.CreateUserOnlyAsync(context, firstName, lastName, null!, null, adminId == 0 ? null : adminId);
                        batchUsers.Add((user, canHoId));
                    }
                }

                DatabaseSeeder.ClearAllDomainEvents(context);
                await context.SaveChangesAsync();

                foreach (var item in batchUsers)
                {
                    var headJoinDate = headApartmentDates[item.CanHoId];
                    // "Other" joins between head's join date and today
                    var daysSinceHeadJoined = (int)(DateTimeOffset.Now - headJoinDate).TotalDays;
                    var joinDate = headJoinDate.AddDays(faker.Random.Number(0, daysSinceHeadJoined));

                    var qh = new QuanHeCuTru(item.CanHoId, item.User.Id, faker.PickRandom(otherRoles), joinDate);
                    if (adminId != 0) qh.SetCreated(adminId, joinDate);
                    context.QuanHeCuTrus.Add(qh);
                }

                DatabaseSeeder.ClearAllDomainEvents(context);
                await context.SaveChangesAsync();
            }
        }

        logger.LogInformation("Finished seeding QuanHeCuTrus.");
    }
}
