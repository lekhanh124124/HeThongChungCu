using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Authentication;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class UserSeeder
{
    private static readonly object _lock = new();
    private static readonly HasherService _passwordHasher = new();
    private static readonly HashSet<string> _usedEmails = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> _usedPhoneNumbers = new();
    private static readonly HashSet<string> _usedIdCards = new();
    private static readonly HashSet<string> _usedUsernames = new(StringComparer.OrdinalIgnoreCase);

    public static async Task InitializeAsync(AppDbContext context)
    {
        lock (_lock)
        {
            _usedEmails.Clear();
            _usedPhoneNumbers.Clear();
            _usedIdCards.Clear();
            _usedUsernames.Clear();
        }

        var existingEmails = await context.TaiKhoan.AsNoTracking().IgnoreQueryFilters().Select(a => a.Email).ToListAsync();
        var existingUsernames = await context.TaiKhoan.AsNoTracking().IgnoreQueryFilters().Select(a => a.TenDangNhap).ToListAsync();
        var existingPhones = await context.NguoiDung.AsNoTracking().IgnoreQueryFilters().Select(u => u.SoDienThoai).ToListAsync();
        var existingIdCards = await context.NguoiDung.AsNoTracking().IgnoreQueryFilters().Select(u => u.CCCD).ToListAsync();

        lock (_lock)
        {
            foreach (var email in existingEmails) if (email != null && !string.IsNullOrEmpty(email.Value)) _usedEmails.Add(email.Value.ToLower());
            foreach (var username in existingUsernames) if (!string.IsNullOrEmpty(username)) _usedUsernames.Add(username.ToLower());
            foreach (var phone in existingPhones) if (!string.IsNullOrEmpty(phone)) _usedPhoneNumbers.Add(phone);
            foreach (var idCard in existingIdCards) if (!string.IsNullOrEmpty(idCard)) _usedIdCards.Add(idCard);
        }
    }

    public static void RegisterSpecialValues()
    {
        lock (_lock)
        {
            // Admin & Test Accounts
            _usedEmails.Add("admin@gmail.com");
            _usedEmails.Add("phognguen0@gmail.com");
            _usedEmails.Add("nhanvien@gmail.com");
            _usedUsernames.Add("admin");
            _usedUsernames.Add("banquanly_test");
            _usedUsernames.Add("nhanvien_test");

            // Special Users (Giang Kiet, Hong Phat)
            _usedEmails.Add("giangkiet2k4@gmail.com");
            _usedUsernames.Add("giangkiet2k4");
            _usedIdCards.Add("001004123456");
            _usedPhoneNumbers.Add("0912345678");

            _usedEmails.Add("hongphat@gmail.com");
            _usedUsernames.Add("hongphat");
            _usedIdCards.Add("001004987654");
            _usedPhoneNumbers.Add("0987654321");
        }
    }

    public static string GetUniqueIdCard(string? pattern = "0010########")
    {
        var faker = new Bogus.Faker();
        string idCard;
        lock (_lock)
        {
            do
            {
                idCard = faker.Random.Replace(pattern ?? "0010########");
            } while (!_usedIdCards.Add(idCard));
        }
        return idCard;
    }

    public static string GetUniquePhoneNumber(string? pattern = "09########")
    {
        var faker = new Bogus.Faker();
        string phone;
        lock (_lock)
        {
            do
            {
                phone = faker.Phone.PhoneNumber(pattern ?? "09########");
            } while (!_usedPhoneNumbers.Add(phone));
        }
        return phone;
    }

    public static string EnsureUniqueEmail(string email)
    {
        var originalEmail = email.ToLower();
        var currentEmail = originalEmail;
        int counter = 1;

        lock (_lock)
        {
            while (!_usedEmails.Add(currentEmail))
            {
                var parts = originalEmail.Split('@');
                currentEmail = $"{parts[0]}{counter}@{parts[1]}";
                counter++;
            }
        }
        return currentEmail;
    }

    public static string EnsureUniqueUsername(string username)
    {
        var originalUsername = username.ToLower();
        var currentUsername = originalUsername;
        int counter = 1;

        lock (_lock)
        {
            while (!_usedUsernames.Add(currentUsername))
            {
                currentUsername = $"{originalUsername}{counter}";
                counter++;
            }
        }
        return currentUsername;
    }

    public static string RegisterEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return email;
        lock (_lock)
        {
            _usedEmails.Add(email.ToLower());
        }
        return email;
    }

    public static string RegisterUsername(string username)
    {
        if (string.IsNullOrEmpty(username)) return username;
        lock (_lock)
        {
            _usedUsernames.Add(username.ToLower());
        }
        return username;
    }

    public static string RegisterPhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone)) return phone;
        lock (_lock)
        {
            _usedPhoneNumbers.Add(phone);
        }
        return phone;
    }

    public static string RegisterIdCard(string idCard)
    {
        if (string.IsNullOrEmpty(idCard)) return idCard;
        lock (_lock)
        {
            _usedIdCards.Add(idCard);
        }
        return idCard;
    }

    public static async Task SeedAdminAndTestAccountsAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.TaiKhoan.AnyAsync(a => a.TenDangNhap == "admin@gmail.com"))
        {
            logger.LogInformation("Seeding Admin and Test Accounts...");

            var testData = new[]
            {
                (Username: "admin", Email: "admin@gmail.com", Role: Role.Admin, FirstName: "Quản trị", LastName: "Hệ thống"),
                (Username: "banquanly_test", Email: "phognguen0@gmail.com", Role: Role.Manager, FirstName: "Ban", LastName: "Quản lý"),
                (Username: "nhanvien_test", Email: "nhanvien@gmail.com", Role: Role.Staff, FirstName: "Trần", LastName: "Nhân Viên")
            };

            foreach (var data in testData)
            {
                var email = RegisterEmail(data.Email);
                var username = RegisterUsername(data.Username);

                await CreateUserWithAccountAsync(
                    context: context,
                    firstName: data.FirstName,
                    lastName: data.LastName,
                    email: email,
                    role: data.Role,
                    phoneNumber: GetUniquePhoneNumber(),
                    address: "TP. Hồ Chí Minh",
                    username: username);
            }
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
        string address = "Hồ Chí Minh",
        string? username = null)
    {
        var user = await CreateUserOnlyAsync(context, firstName, lastName, phoneNumber, address);

        var finalUsername = string.IsNullOrEmpty(username) ? email : username;
        var account = new TaiKhoan(user.Id, finalUsername, email, _passwordHasher.HashPassword("123456"));
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
            RegisterIdCard(GetUniqueIdCard()),
            phoneNumber ?? RegisterPhoneNumber(GetUniquePhoneNumber()));

        await context.NguoiDung.AddAsync(user);

        // We MUST save changes here to get user.Id
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
