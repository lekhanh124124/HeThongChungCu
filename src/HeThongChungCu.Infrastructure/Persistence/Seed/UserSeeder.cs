using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Authentication;
using HeThongChungCu.Infrastructure.Persistence;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class UserSeeder
{
    private static readonly PasswordHasher _passwordHasher = new();
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int count)
    {
        if (!await context.Users.AnyAsync())
        {
            logger.LogInformation("Seeding Users...");

            var userFaker = new Bogus.Faker<User>("vi")
                .CustomInstantiator(f => new User(
                    username: f.Internet.UserName(),
                    email: f.Internet.Email(),
                    passwordHash: _passwordHasher.HashPassword("123456"),
                    firstName: f.Name.FirstName(),
                    lastName: f.Name.LastName(),
                    phoneNumber: f.Phone.PhoneNumber("0#########"),
                    idCard: f.Random.Replace("0010########"),
                    dob: f.Date.PastOffset(30, DateTime.Now.AddYears(-18)).Date,
                    gioiTinhId: f.PickRandom(GioiTinh.GetAll().ToArray()),
                    diaChi: f.Address.FullAddress()
                ))
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("0#########"));

            var users = new List<User>();
            var phoneNumbers = new HashSet<string>();

            // Generate unique phone numbers
            while (users.Count < count)
            {
                var user = userFaker.Generate();
                if (phoneNumbers.Add(user.PhoneNumber))
                {
                    users.Add(user);
                }
            }

            // Hardcode 1 admin for easy login
            if (users.Count > 0)
            {
                users[0] = new User(
                    "admin",
                    "admin@gmail.com",
                    _passwordHasher.HashPassword("123456"),
                    "Admin",
                    "System",
                    "0987654321",
                    "001090123456",
                    new DateTime(1990, 1, 1),
                    GioiTinh.Nam,
                    "Hà Nội");
                users[0].ChangeRole(Role.Admin);

                users[1] = new User(
                    "banquanly_test",
                    "banquanly_test@gmail.com",
                    _passwordHasher.HashPassword("123456"),
                    "Ban Quản Lý",
                    "System",
                    "0987654322",
                    "001090123457",
                    new DateTime(1990, 1, 1),
                    GioiTinh.Nam,
                    "Hà Nội");
                users[1].ChangeRole(Role.Manager);

                users[2] = new User(
                    "cudan_test",
                    "cudan_test@gmail.com",
                    _passwordHasher.HashPassword("123456"),
                    "Cư Dân",
                    "System",
                    "0987654323",
                    "001090123458",
                    new DateTime(1990, 1, 1),
                    GioiTinh.Nam,
                    "Hà Nội");
                users[2].ChangeRole(Role.Resident);

                users[3] = new User(
                    "nhanvien_test",
                    "nhanvien_test@gmail.com",
                    _passwordHasher.HashPassword("123456"),
                    "Nhân viên",
                    "System",
                    "0987654324",
                    "001090123459",
                    new DateTime(1990, 1, 1),
                    GioiTinh.Nam,
                    "Hà Nội");
                users[3].ChangeRole(Role.Staff);

                users[4] = new User(
                    "khach_test",
                    "khach_test@gmail.com",
                    _passwordHasher.HashPassword("123456"),
                    "Khách",
                    "System",
                    "0987654325",
                    "001090123460",
                    new DateTime(1990, 1, 1),
                    GioiTinh.Nam,
                    "Hà Nội");
                users[4].ChangeRole(Role.Guest);

                for (int i = 5; i < users.Count; i++)
                {
                    users[i].ChangeRole(Role.Resident);
                }
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
