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
    private static readonly HashSet<string> _usedPhoneNumbers = [];
    private static readonly HashSet<string> _usedIdCards = [];
    private static readonly HashSet<string> _usedUsernames = new(StringComparer.OrdinalIgnoreCase);

    public static readonly string[] VietnamAddresses = new[]
    {
        "15 Lê Thánh Tôn, Bến Nghé, Quận 1, TP. Hồ Chí Minh",
        "200 Nguyễn Thị Minh Khai, Võ Thị Sáu, Quận 3, TP. Hồ Chí Minh",
        "456 Trần Hưng Đạo, Cầu Kho, Quận 1, TP. Hồ Chí Minh",
        "12/4 Phan Xích Long, Phường 2, Quận Phú Nhuận, TP. Hồ Chí Minh",
        "Số 1 Nguyễn Huệ, Bến Nghé, Quận 1, TP. Hồ Chí Minh",
        "78 Lê Lợi, Bến Thành, Quận 1, TP. Hồ Chí Minh",
        "102 Cách Mạng Tháng 8, Võ Thị Sáu, Quận 3, TP. Hồ Chí Minh",
        "99 Trần Não, An Khánh, TP. Thủ Đức, TP. Hồ Chí Minh",
        "246 Võ Văn Kiệt, Cô Giang, Quận 1, TP. Hồ Chí Minh",
        "55 Pasteur, Bến Nghé, Quận 1, TP. Hồ Chí Minh",
        "150 Lý Tự Trọng, Bến Thành, Quận 1, TP. Hồ Chí Minh",
        "88 Đồng Khởi, Bến Nghé, Quận 1, TP. Hồ Chí Minh",
        "300 Ba Tháng Hai, Phường 12, Quận 10, TP. Hồ Chí Minh",
        "50 Đinh Tiên Hoàng, Đa Kao, Quận 1, TP. Hồ Chí Minh",
        "67 Nam Kỳ Khởi Nghĩa, Võ Thị Sáu, Quận 3, TP. Hồ Chí Minh",
        "210 Điện Biên Phủ, Võ Thị Sáu, Quận 3, TP. Hồ Chí Minh",
        "45 Thảo Điền, Tháp Mười, TP. Thủ Đức, TP. Hồ Chí Minh",
        "123 Lê Văn Sỹ, Phường 13, Quận Phú Nhuận, TP. Hồ Chí Minh"
    };

    public static string GetRandomVietnamAddress()
    {
        var faker = new Bogus.Faker();
        return faker.PickRandom(VietnamAddresses);
    }

    public static async Task ResetAndSyncAsync(AppDbContext context)
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

            // Register Special Values (Placeholders for upcoming fixed seeds)
            _usedEmails.Add("admin@gmail.com");
            _usedEmails.Add("phognguen0@gmail.com");
            _usedEmails.Add("nhanvien@gmail.com");
            _usedEmails.Add("giangkiet2k4@gmail.com");
            _usedEmails.Add("hongphat@gmail.com");

            _usedUsernames.Add("admin");
            _usedUsernames.Add("banquanly_test");
            _usedUsernames.Add("nhanvien_test");
            _usedUsernames.Add("giangkiet2k4");
            _usedUsernames.Add("hongphat");

            _usedIdCards.Add("001004123456");
            _usedIdCards.Add("001004987654");

            _usedPhoneNumbers.Add("0912345678");
            _usedPhoneNumbers.Add("0987654321");
        }
    }

    public static string GetUniqueIdCard(int? gioiTinhId = null, int? birthYear = null)
    {
        var faker = new Bogus.Faker();
        string idCard;

        lock (_lock)
        {
            do
            {
                // 1. Mã tỉnh (001-096) - Lấy mẫu một số tỉnh lớn
                var provinceCodes = new[] { "001", "048", "079", "031", "075", "064", "030", "036", "040", "051" };
                var province = faker.PickRandom(provinceCodes);

                // 2. Mã giới tính + thế kỷ
                // - Thế kỷ 20 (1900-1999): Nam 0, Nữ 1
                // - Thế kỷ 21 (2000-2099): Nam 2, Nữ 3
                int genderCode;
                int year = birthYear ?? faker.Date.Past(50, DateTime.Now.AddYears(-20)).Year;
                bool isMale = gioiTinhId == null || gioiTinhId == 1; // Giả định 1 là Nam

                if (year < 2000) genderCode = isMale ? 0 : 1;
                else genderCode = isMale ? 2 : 3;

                // 3. 2 số cuối năm sinh
                var yearStr = (year % 100).ToString("D2");

                // 4. 6 số ngẫu nhiên
                var randomDigits = faker.Random.Number(100000, 999999).ToString();

                idCard = $"{province}{genderCode}{yearStr}{randomDigits}";

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

    public static async Task SeedGuestAccountsAsync(AppDbContext context, ILogger logger, int count, int adminId)
    {
        logger.LogInformation("Seeding {Count} Guest Accounts...", count);
        var faker = new Bogus.Faker("vi");
        var hashedPassword = _passwordHasher.HashPassword("123456");

        for (int i = 0; i < count; i++)
        {
            var email = EnsureUniqueEmail(faker.Internet.Email().ToLower());
            var account = new TaiKhoan(null, email, email, hashedPassword);
            account.AddRole(Role.Guest);
            account.SetCreated(adminId, DateTimeOffset.Now);
            await context.TaiKhoan.AddAsync(account);
        }

        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();
    }

    public static async Task<(NguoiDung NguoiDung, TaiKhoan TaiKhoan)> CreateUserWithAccountAsync(
        AppDbContext context,
        string firstName,
        string lastName,
        string email,
        Role role,
        string phoneNumber,
        string? address = null,
        string? username = null,
        int? createdBy = null)
    {
        var user = await CreateUserOnlyAsync(context, firstName, lastName, phoneNumber, address, createdBy);

        var finalUsername = string.IsNullOrEmpty(username) ? email : username;
        var account = new TaiKhoan(user.Id, finalUsername, email, _passwordHasher.HashPassword("123456"));
        account.AddRole(role);

        if (createdBy.HasValue)
        {
            account.SetCreated(createdBy.Value, DateTimeOffset.Now);
        }

        await context.TaiKhoan.AddAsync(account);
        DatabaseSeeder.ClearAllDomainEvents(context);
        await context.SaveChangesAsync();

        return (user, account);
    }

    public static async Task<NguoiDung> CreateUserOnlyAsync(
        AppDbContext context,
        string firstName,
        string lastName,
        string phoneNumber,
        string? address = null,
        int? createdBy = null)
    {
        var faker = new Bogus.Faker("vi");

        var dob = faker.Date.PastOffset(40, DateTimeOffset.Now.AddYears(-20));
        var gioiTinh = faker.PickRandom(GioiTinh.GetAll().ToArray());

        var user = new NguoiDung(
            firstName,
            lastName,
            dob,
            gioiTinh,
            string.IsNullOrWhiteSpace(address) || address == "Hồ Chí Minh" || address == "TP. Hồ Chí Minh"
                ? GetRandomVietnamAddress()
                : address,
            RegisterIdCard(GetUniqueIdCard(gioiTinh.Value, dob.Year)),
            phoneNumber ?? RegisterPhoneNumber(GetUniquePhoneNumber()));

        if (createdBy.HasValue)
        {
            user.SetCreated(createdBy.Value, DateTimeOffset.Now);
        }

        await context.NguoiDung.AddAsync(user);

        // We MUST save changes here to get user.Id
        DatabaseSeeder.ClearAllDomainEvents(context);
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
