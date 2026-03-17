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

                for (int i = 1; i < users.Count; i++)
                {
                    users[i].ChangeRole(Role.Resident);
                }
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
