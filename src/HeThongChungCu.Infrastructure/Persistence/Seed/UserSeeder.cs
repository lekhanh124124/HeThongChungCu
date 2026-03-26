using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Authentication;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class UserSeeder
{
    private static readonly HasherService _passwordHasher = new();
    private static readonly HashSet<string> _usedEmails = new();
    private static readonly HashSet<string> _usedPhoneNumbers = new();
    private static readonly HashSet<string> _usedIdCards = new();

    private static string GetUniqueIdCard()
    {
        var faker = new Bogus.Faker();
        string idCard;
        do
        {
            idCard = faker.Random.Replace("0010########");
        } while (!_usedIdCards.Add(idCard));
        return idCard;
    }

    private static string GetUniquePhoneNumber()
    {
        var faker = new Bogus.Faker();
        string phone;
        do
        {
            phone = faker.Phone.PhoneNumber("09########");
        } while (!_usedPhoneNumbers.Add(phone));
        return phone;
    }

    private static string EnsureUniqueEmail(string email)
    {
        var originalEmail = email.ToLower();
        var currentEmail = originalEmail;
        int counter = 1;
        
        while (!_usedEmails.Add(currentEmail))
        {
            var parts = originalEmail.Split('@');
            currentEmail = $"{parts[0]}{counter}@{parts[1]}";
            counter++;
        }
        return currentEmail;
    }

    public static string RegisterEmail(string email)
    {
        _usedEmails.Add(email.ToLower());
        return email;
    }

    public static string RegisterPhoneNumber(string phone)
    {
        _usedPhoneNumbers.Add(phone);
        return phone;
    }

    public static string RegisterIdCard(string idCard)
    {
        _usedIdCards.Add(idCard);
        return idCard;
    }

    public static async Task SeedAdminAndTestAccountsAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.TaiKhoan.AnyAsync(a => a.TenDangNhap == "admin@gmail.com"))
        {
            logger.LogInformation("Seeding Admin and Test Accounts...");
            var hashedPassword = _passwordHasher.HashPassword("123456");

            var testData = new[]
            {
                (Email: "admin@gmail.com", Role: Role.Admin, FirstName: "Quản trị", LastName: "Hệ thống"),
                (Email: "phognguen0@gmail.com", Role: Role.Manager, FirstName: "Ban", LastName: "Quản lý"),
                (Email: "nhanvien@gmail.com", Role: Role.Staff, FirstName: "Trần", LastName: "Nhân Viên")
            };

            foreach (var data in testData)
            {
                var user = new NguoiDung(
                    data.FirstName,
                    data.LastName,
                    new DateTime(1985, 5, 20),
                    GioiTinh.Nam,
                    "TP. Hồ Chí Minh",
                    GetUniqueIdCard(),
                    GetUniquePhoneNumber());

                await context.NguoiDung.AddAsync(user);
                await context.SaveChangesAsync();

                var email = EnsureUniqueEmail(data.Email);
                var account = new TaiKhoan(user.Id, email, email, hashedPassword);
                account.AddRole(data.Role);
                await context.TaiKhoan.AddAsync(account);
            }
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedGuestAccountsAsync(AppDbContext context, ILogger logger, int count)
    {
        logger.LogInformation("Seeding {Count} Guest Accounts...", count);
        var faker = new Bogus.Faker("vi");
        var hashedPassword = _passwordHasher.HashPassword("123456");

        for (int i = 0; i < count; i++)
        {
            var email = EnsureUniqueEmail(faker.Internet.Email().ToLower());
            var account = new TaiKhoan(null, email, email, hashedPassword);
            account.AddRole(Role.Guest);
            await context.TaiKhoan.AddAsync(account);
        }
        await context.SaveChangesAsync();
    }

    public static async Task<(NguoiDung NguoiDung, TaiKhoan TaiKhoan)> CreateUserWithAccountAsync(
        AppDbContext context,
        string firstName,
        string lastName,
        string email,
        Role role,
        string phoneNumber,
        string address = "Hồ Chí Minh")
    {
        var user = new NguoiDung(
            firstName,
            lastName,
            new DateTime(1990, 1, 1),
            GioiTinh.Nam,
            address,
            GetUniqueIdCard(),
            phoneNumber ?? GetUniquePhoneNumber());

        await context.NguoiDung.AddAsync(user);
        await context.SaveChangesAsync();

        var account = new TaiKhoan(user.Id, email, email, _passwordHasher.HashPassword("123456"));
        account.AddRole(role);
        await context.TaiKhoan.AddAsync(account);
        await context.SaveChangesAsync();

        return (user, account);
    }

    public static async Task<NguoiDung> CreateUserOnlyAsync(
        AppDbContext context,
        string firstName,
        string lastName,
        string phoneNumber,
        string address = "Hồ Chí Minh")
    {
        var user = new NguoiDung(
            firstName,
            lastName,
            new DateTime(1995, 1, 1),
            GioiTinh.Nam,
            address,
            GetUniqueIdCard(),
            phoneNumber ?? GetUniquePhoneNumber());

        await context.NguoiDung.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public static string GenerateEmailFromName(string firstName, string lastName)
    {
        var emailPrefix = StringUtils.RemoveDiacritics($"{firstName}.{lastName}").ToLower().Replace(" ", "");
        return EnsureUniqueEmail($"{emailPrefix}@gmail.com");
    }

    public static class StringUtils
    {
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}
