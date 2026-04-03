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
        await SeedSpecificUserScenarioAsync(
            context, logger, "Tuấn Kiệt", "Giang", new DateTime(2004, 1, 1), GioiTinh.Nam,
            "TP. Hồ Chí Minh", "001004123456", "0912345678", "giangkiet2k4@gmail.com", "giangkiet2k4@gmail.com");
    }

    public static async Task SeedHongPhatAsync(AppDbContext context, ILogger logger)
    {
        await SeedSpecificUserScenarioAsync(
            context, logger, "Hồng Phát", "Nguyễn", new DateTime(1995, 5, 20), GioiTinh.Nam,
            "Đà Nẵng", "001004987654", "0987654321", "hongphat@gmail.com", "hongphat");
    }

    private static async Task SeedSpecificUserScenarioAsync(
        AppDbContext context,
        ILogger logger,
        string firstName,
        string lastName,
        DateTime dob,
        GioiTinh gender,
        string diaChi,
        string cccd,
        string phone,
        string emailInput,
        string username)
    {
        logger.LogInformation($"Seeding Special User: {lastName} {firstName}...");

        var adminAccount = await context.TaiKhoan
            .FirstOrDefaultAsync(a => a.Email == "admin@gmail.com");

        var canHos = await context.CanHos.ToListAsync();
        if (canHos.Count < 4)
        {
            logger.LogWarning("Not enough apartments to seed special user case.");
            return;
        }

        var faker = new Faker("vi");

        // 1. Create Special User and Account
        var user = new NguoiDung(
            firstName,
            lastName,
            dob,
            gender,
            diaChi,
            UserSeeder.RegisterIdCard(cccd),
            UserSeeder.RegisterPhoneNumber(phone));

        await context.NguoiDung.AddAsync(user);
        await context.SaveChangesAsync();

        var email = UserSeeder.RegisterEmail(emailInput);
        var account = new TaiKhoan(user.Id, username, email, _passwordHasher.HashPassword("123456."));
        account.AddRole(Role.Resident);
        await context.TaiKhoan.AddAsync(account);
        await context.SaveChangesAsync();

        // 2. Pick 4 apartments that are not already occupied by this specific user
        // We'll pick from available or random ones
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
            var qh = new QuanHeCuTru(canHo.Id, user.Id, LoaiQuanHeCuTru.ChuHo, DateTime.Now.AddMonths(-6));

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

                var rqh = new QuanHeCuTru(canHo.Id, rUser.Id, faker.PickRandom(otherRoles), DateTime.Now.AddMonths(-5));

                context.QuanHeCuTrus.Add(rqh);
            }
            await context.SaveChangesAsync();

            // 4. Add 2 vehicles and 4 cards total per apartment
            for (int k = 0; k < 2; k++)
            {
                var loai = k == 0 ? LoaiPhuongTien.Oto : LoaiPhuongTien.XeMay;
                var model = k == 0 ? "Toyota Camry" : "Honda SH";
                var bienSo = PhuongTienSeeder.RegisterBienSo($"{faker.Random.Int(29, 31)}{faker.Random.String2(1, "ABCDEFGHJK")}-{faker.Random.Int(10000, 99999)}");

                var pt = new PhuongTien(canHo.Id, model, loai, bienSo, model, null);
                await context.PhuongTiens.AddAsync(pt);
                await context.SaveChangesAsync();

                // 2 cards per vehicle (Total 4 per apartment)
                for (int c = 0; c < 2; c++)
                {
                    var maThe = PhuongTienSeeder.RegisterMaThe(faker.Random.Replace("SP-##########"));
                    var the = pt.AddThe(maThe, DateTime.Now.AddMonths(-4));

                    // If we add multiple cards, the older ones must be locked (Business Rule)
                    if (c == 0)
                    {
                        the.KhoaThe(DateTime.Now.AddMonths(-3));
                    }
                }
                await context.SaveChangesAsync();
            }

            // 5. Add 2 residency requests and 2 vehicle requests for this apartment
            // Residency Request 1: Them member (Approved if active)
            var req1 = YeuCauCuTru.CreateAddMemberRequest(
                canHo.Id,
                null,
                LoaiQuanHeCuTru.NguoiOCung.Value,
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Date.Past(20, DateTime.Now.AddYears(-18)),
                1,
                faker.Phone.PhoneNumber("09########"),
                faker.Random.Replace("0010########"),
                faker.Address.FullAddress(),
                "Thêm thành viên vào cư trú cùng.",
                null,
                isTerminated ? TrangThaiYeuCau.Invalidated : TrangThaiYeuCau.Pending);

            if (!isTerminated && adminAccount != null)
            {
                req1.Approve(adminAccount.Id, DateTimeOffset.Now.AddMonths(-1));
            }
            await context.YeuCauCuTrus.AddAsync(req1);

            // Residency Request 2: Xoa member (Pending if active)
            var req2 = YeuCauCuTru.CreateRemoveMemberRequest(
                canHo.Id,
                user.Id,
                "Yêu cầu xóa thành viên do chuyển đi.",
                isTerminated ? TrangThaiYeuCau.Invalidated : TrangThaiYeuCau.Pending);
            await context.YeuCauCuTrus.AddAsync(req2);

            // Vehicle Request 1: Them vehicle (Approved if active)
            var vreq1 = YeuCauPhuongTien.CreateAddRequest(
                canHo.Id,
                LoaiPhuongTien.Oto,
                "Mercedes-Benz C200",
                faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                "Sliver",
                "Đăng ký xe mới cho gia đình.",
                null,
                isTerminated ? TrangThaiYeuCau.Invalidated : TrangThaiYeuCau.Pending);

            if (!isTerminated && adminAccount != null)
            {
                vreq1.Approve(adminAccount.Id, DateTimeOffset.Now.AddMonths(-2));
            }
            await context.YeuCauPhuongTiens.AddAsync(vreq1);

            // Vehicle Request 2: Sua vehicle (Rejected if active)
            var vreq2 = YeuCauPhuongTien.CreateUpdateRequest(
                canHo.Id,
                faker.Random.Int(10, 1000), // Dummy vehicle ID
                LoaiPhuongTien.XeMay,
                "Honda Vision",
                faker.Vehicle.Vin().Substring(0, 8).ToUpper(),
                "Red",
                "Cập nhật lại màu sắc xe.",
                null,
                isTerminated ? TrangThaiYeuCau.Invalidated : TrangThaiYeuCau.Pending);

            if (!isTerminated && adminAccount != null)
            {
                vreq2.Reject(adminAccount.Id, "Sai thông tin biển số xe.", DateTimeOffset.Now.AddMonths(-1));
            }
            await context.YeuCauPhuongTiens.AddAsync(vreq2);
            await context.SaveChangesAsync();
        }

        logger.LogInformation($"Finished seeding Special User: {lastName} {firstName}.");
    }
}
