using Bogus;
using HeThongChungCu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class UserSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.Users.AnyAsync())
        {
            logger.LogInformation("Seeding Users...");

            var userFaker = new Faker<User>("vi")
                .CustomInstantiator(f => new User(
                    username: f.Internet.UserName(),
                    email: f.Internet.Email(),
                    passwordHash: "$2a$11$N/zE3y5Z.I1xL0d7p4kCpe4H6.Q95I5xT8bI.H8bM97m8m/y0f1m2", // Admin@123
                    firstName: f.Name.FirstName(),
                    lastName: f.Name.LastName(),
                    phoneNumber: f.Phone.PhoneNumber("0#########"),
                    idCard: f.Random.Replace("0010########"),
                    dob: f.Date.PastOffset(30, DateTime.Now.AddYears(-18)).Date,
                    gioiTinhId: f.Random.Int(1, 2),
                    diaChi: f.Address.FullAddress()
                ));

            var users = userFaker.Generate(10);

            // Hardcode 1 admin for easy login
            users[0] = new User("admin", "admin@example.com", "$2a$11$N/zE3y5Z.I1xL0d7p4kCpe4H6.Q95I5xT8bI.H8bM97m8m/y0f1m2", "Admin", "System", "0987654321", "001090123456", new DateTime(1990, 1, 1), 1, "Hà Nội");
            users[0].ChangeRole(HeThongChungCu.Domain.Enums.Role.Admin);

            for (int i = 1; i < users.Count; i++)
            {
                users[i].ChangeRole(HeThongChungCu.Domain.Enums.Role.Resident);
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
