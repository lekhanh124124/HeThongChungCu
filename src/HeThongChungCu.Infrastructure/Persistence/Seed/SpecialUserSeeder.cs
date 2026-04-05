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
        if (await context.TaiKhoan.IgnoreQueryFilters().AnyAsync(a => a.Email.Value == "giangkiet2k4@gmail.com"))
        {
            logger.LogInformation("Special User Giang Kiet already exists. Skipping.");
            return;
        }

        await SeedSpecificUserScenarioAsync(
            context, logger, "Tuấn Kiệt", "Giang", new DateTimeOffset(new DateTime(2004, 1, 1)), GioiTinh.Nam,
            "TP. Hồ Chí Minh", "001004123456", "0912345678", "giangkiet2k4@gmail.com", "giangkiet2k4@gmail.com");
    }

    public static async Task SeedHongPhatAsync(AppDbContext context, ILogger logger)
    {
        if (await context.TaiKhoan.IgnoreQueryFilters().AnyAsync(a => a.Email.Value == "hongphat@gmail.com"))
        {
            logger.LogInformation("Special User Hong Phat already exists. Skipping.");
            return;
        }

        await SeedSpecificUserScenarioAsync(
            context, logger, "Hồng Phát", "Nguyễn", new DateTimeOffset(new DateTime(1995, 5, 20)), GioiTinh.Nam,
            "Đà Nẵng", "001004987654", "0987654321", "hongphat@gmail.com", "hongphat");
    }

    private static async Task SeedSpecificUserScenarioAsync(
        AppDbContext context,
        ILogger logger,
        string firstName,
        string lastName,
        DateTimeOffset dob,
        GioiTinh gender,
        string diaChi,
        string cccd,
        string phone,
        string emailInput,
        string username)
    {
        logger.LogInformation($"Seeding Special User: {lastName} {firstName}...");

        var adminAccount = await context.TaiKhoan.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");

        var canHos = await context.CanHos.ToListAsync();
        if (canHos.Count < 4)
        {
            logger.LogWarning("Not enough apartments to seed special user case.");
            return;
        }

        var faker = new Faker("vi");

        // 1. Create Special User and Account (No SaveChanges yet)
        var user = new NguoiDung(
            firstName,
            lastName,
            dob,
            gender,
            diaChi,
            UserSeeder.RegisterIdCard(cccd),
            UserSeeder.RegisterPhoneNumber(phone));

        await context.NguoiDung.AddAsync(user);

        // We MUST save changes here to get user.Id for the TaiKhoan link 
        // if we are not using navigation properties
        await context.SaveChangesAsync();

        var email = UserSeeder.RegisterEmail(emailInput);
        var account = new TaiKhoan(user.Id, UserSeeder.RegisterUsername(username), email, _passwordHasher.HashPassword("123456."));
        account.AddRole(Role.Resident);
        await context.TaiKhoan.AddAsync(account);

        // 2. Pick 4 apartments
        var selectedCanHos = canHos.OrderBy(x => Guid.NewGuid()).Take(4).ToList();
        var canHoIds = selectedCanHos.Select(c => c.Id).ToList();

        // Bulk fetch relations for these apartments
        var allRelations = await context.QuanHeCuTrus
            .Where(r => canHoIds.Contains(r.CanHoId) && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru)
            .ToListAsync();

        var relationsByCanHo = allRelations.GroupBy(r => r.CanHoId).ToDictionary(g => g.Key, g => g.ToList());

        for (int i = 0; i < selectedCanHos.Count; i++)
        {
            var canHo = selectedCanHos[i];

            // Terminate existing active head (ChuHo or NguoiThue) if any
            if (relationsByCanHo.TryGetValue(canHo.Id, out var activeHeads))
            {
                foreach (var h in activeHeads.Where(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue))
                {
                    h.KetThucCuTru(DateTimeOffset.UtcNow);
                }
            }

            // 3 active, 1 terminated
            var isTerminated = i == 3;
            var qh = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.ChuHo, DateTimeOffset.UtcNow.AddMonths(-6));

            if (isTerminated)
            {
                qh.KetThucCuTru(DateTimeOffset.UtcNow.AddMonths(-1));
            }

            context.QuanHeCuTrus.Add(qh);

            if (!isTerminated)
            {
                canHo.MarkAsOccupied();
            }

            // 3. Add 3-5 residents per apartment
            int residentCount = faker.Random.Number(3, 5);
            var otherRoles = new[] { LoaiQuanHeCuTru.NguoiOCung }; // Only add as co-residents to follow the rules

            for (int j = 0; j < residentCount; j++)
            {
                var rFirstName = faker.Name.FirstName();
                var rLastName = faker.Name.LastName();
                var rUser = await UserSeeder.CreateUserOnlyAsync(context, rFirstName, rLastName, faker.Phone.PhoneNumber("09########"), "Hồ Chí Minh");

                var rqh = new QuanHeCuTru(canHo.Id, rUser.Id, faker.PickRandom(otherRoles), DateTimeOffset.UtcNow.AddMonths(-5));
                context.QuanHeCuTrus.Add(rqh);
            }

            // 4. Add 2 vehicles and 4 cards total per apartment
            for (int k = 0; k < 2; k++)
            {
                var loai = k == 0 ? LoaiPhuongTien.Oto : LoaiPhuongTien.XeMay;
                var model = k == 0 ? "Toyota Camry" : "Honda SH";
                var bienSo = PhuongTienSeeder.RegisterBienSo($"{faker.Random.Int(29, 31)}{faker.Random.String2(1, "ABCDEFGHJK")}-{faker.Random.Int(10000, 99999)}");

                var pt = new PhuongTien(canHo.Id, model, loai, bienSo, model, null);
                await context.PhuongTiens.AddAsync(pt);

                // 2 cards per vehicle (Total 4 per apartment)
                for (int c = 0; c < 2; c++)
                {
                    var maThe = PhuongTienSeeder.RegisterMaThe(faker.Random.Replace("SP-##########"));
                    var the = pt.AddThe(maThe, DateTimeOffset.UtcNow.AddMonths(-4));

                    if (c == 0)
                    {
                        the.KhoaThe(DateTimeOffset.UtcNow.AddMonths(-3));
                    }
                }
            }

            // 5. Add 2 residency requests and 2 vehicle requests
            var req1Status = isTerminated ? TrangThaiYeuCau.Invalidated : TrangThaiYeuCau.Pending;
            var req1 = YeuCauCuTru.CreateAddMemberRequest(
                canHo.Id, null, LoaiQuanHeCuTru.NguoiOCung.Value,
                faker.Name.FirstName(), faker.Name.LastName(),
                faker.Date.Past(20, DateTime.Now.AddYears(-18)),
                faker.PickRandom(new[] { 1, 2 }), UserSeeder.GetUniquePhoneNumber(),
                UserSeeder.GetUniqueIdCard(), faker.Address.FullAddress(),
                "Thêm thành viên vào cư trú cùng.", null, req1Status);

            if (!isTerminated && adminAccount != null)
            {
                req1.Approve(adminAccount.Id, DateTimeOffset.UtcNow.AddMonths(-1));
            }
            await context.YeuCauCuTrus.AddAsync(req1);

            var req2 = YeuCauCuTru.CreateRemoveMemberRequest(
                canHo.Id, user.Id, "Yêu cầu xóa thành viên do chuyển đi.",
                isTerminated ? TrangThaiYeuCau.Invalidated : TrangThaiYeuCau.Pending);
            await context.YeuCauCuTrus.AddAsync(req2);

            var vreq1 = YeuCauPhuongTien.CreateAddRequest(
                canHo.Id, LoaiPhuongTien.Oto, "Mercedes-Benz C200",
                faker.Vehicle.Vin().Substring(0, 8).ToUpper(), "Sliver",
                "Đăng ký xe mới cho gia đình.", null, req1Status);

            if (!isTerminated && adminAccount != null)
            {
                vreq1.Approve(adminAccount.Id, DateTimeOffset.UtcNow.AddMonths(-2));
            }
            await context.YeuCauPhuongTiens.AddAsync(vreq1);

            var vreq2 = YeuCauPhuongTien.CreateUpdateRequest(
                canHo.Id, faker.Random.Int(10, 1000), LoaiPhuongTien.XeMay,
                "Honda Vision", faker.Vehicle.Vin().Substring(0, 8).ToUpper(), "Red",
                "Cập nhật lại màu sắc xe.", null, req1Status);

            if (!isTerminated && adminAccount != null)
            {
                vreq2.Reject(adminAccount.Id, "Sai thông tin biển số xe.", DateTimeOffset.UtcNow.AddMonths(-1));
            }
            await context.YeuCauPhuongTiens.AddAsync(vreq2);
        }

        // Final save for all relations and vehicles in this scenario will be handled by the caller or final seeder save
    }
}
