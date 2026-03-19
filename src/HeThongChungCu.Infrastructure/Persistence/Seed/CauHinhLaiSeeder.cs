using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class CauHinhLaiSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.Set<CauHinhLai>().AnyAsync())
        {
            logger.LogInformation("Seeding CauHinhLais...");

            var cauHinh = new CauHinhLai(
                maCauHinh: "CHL-DEFAULT",
                laiSuatThang: 1.5m,
                soNgayChoPhep: 5,
                nguongQuaHanNhe: 30,
                nguongQuaHanNang: 60,
                ngayApDung: new DateTime(2024, 1, 1)
            );

            await context.Set<CauHinhLai>().AddAsync(cauHinh);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded default CauHinhLai.");
        }
    }
}
