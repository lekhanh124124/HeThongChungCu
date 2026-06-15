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
        logger.LogInformation("Seeding ChuHo ({ChuHoCount}) and CuTru ({CuTruCount}) with historical timeline...", soLuongChuHo, soLuongCuTru);

        var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
        var adminId = admin?.Id ?? 0;

        var canHos = await context.CanHos.ToListAsync();
        if (canHos.Count == 0) return;

        var faker = new Faker("vi");
        var availableCanHos = canHos.OrderBy(_ => Guid.NewGuid()).ToList();

        var systemStartDate = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.FromHours(7));
        var systemEndDate = new DateTimeOffset(2026, 4, 30, 23, 59, 59, TimeSpan.FromHours(7));
        var totalSystemDays = (int)(systemEndDate - systemStartDate).TotalDays;

        // 0. Seed Past Residents (DaKetThuc)
        // Lấy 20% số căn hộ để tạo ra những người đã từng ở và chuyển đi
        int pastResidentAptCount = Math.Max(5, availableCanHos.Count / 5);
        var pastApts = availableCanHos.Take(pastResidentAptCount).ToList();

        foreach (var canHo in pastApts)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var email = UserSeeder.GenerateEmailFromName(firstName, lastName);

            (NguoiDung user, _) = await UserSeeder.CreateUserWithAccountAsync(
                context, firstName, lastName, email, Role.Resident, null!, null, null, adminId == 0 ? null : adminId);

            // Sinh ngày vào từ T1/2025 đến khoảng giữa giai đoạn
            var joinDate = systemStartDate.AddDays(faker.Random.Number(0, totalSystemDays / 2));
            // Sinh ngày rời đi từ sau ngày vào đến T4/2026
            var leaveDate = joinDate.AddDays(faker.Random.Number(30, (int)(systemEndDate - joinDate).TotalDays));
            var role = faker.PickRandom(LoaiQuanHeCuTru.ChuHo, LoaiQuanHeCuTru.NguoiThue);
            
            var qh = new QuanHeCuTru(canHo.Id, user.Id, role, joinDate);
            qh.KetThucCuTru(leaveDate); // Đánh dấu đã chuyển đi
            if (adminId != 0) qh.SetCreated(adminId, joinDate);
            context.QuanHeCuTrus.Add(qh);
        }
        
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        // 1. Seed Current Owners (ChuHo)
        int chuHoCount = Math.Min(soLuongChuHo, availableCanHos.Count / 2); // Seed owners for up to half of apartments
        var chuHoCanHos = availableCanHos.Take(chuHoCount).ToList(); // It's ok if they overlap with pastApts
        var headApartmentDates = new Dictionary<int, DateTimeOffset>();

        foreach (var canHo in chuHoCanHos)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var email = UserSeeder.GenerateEmailFromName(firstName, lastName);

            (NguoiDung user, _) = await UserSeeder.CreateUserWithAccountAsync(
                context, firstName, lastName, email, Role.Resident, null!, null, null, adminId == 0 ? null : adminId);

            // Mốc thời gian chuyển vào trải dài từ đầu 2025 đến T4/2026
            var joinDate = systemStartDate.AddDays(faker.Random.Number(0, totalSystemDays));
            var qh = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.ChuHo, joinDate);
            if (adminId != 0) qh.SetCreated(adminId, joinDate);
            context.QuanHeCuTrus.Add(qh);

            canHo.SyncStatusWithResidency(hasOwner: true, hasTenant: false);
            headApartmentDates[canHo.Id] = joinDate;
        }

        // 2. Seed Current Tenants (NguoiThue) for remaining vacant apartments
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

            var joinDate = systemStartDate.AddDays(faker.Random.Number(0, totalSystemDays));
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
