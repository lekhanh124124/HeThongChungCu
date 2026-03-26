using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class PhuongTienSeeder
{
    private static readonly HashSet<string> _usedBienSos = new();
    private static readonly HashSet<string> _usedMaThes = new();

    public static async Task SeedAsync(AppDbContext context, ILogger logger, int soLuongPhuongTien)
    {
        logger.LogInformation("Seeding {Count} PhuongTiens with logic-based cards...", soLuongPhuongTien);

        var canHoIds = await context.CanHos.Select(c => c.Id).ToListAsync();
        if (!canHoIds.Any()) return;

        var faker = new Faker("vi");
        var loaiPhuongTiens = LoaiPhuongTien.GetAll().ToArray();
        var trangThais = TrangThaiPhuongTien.GetAll().ToArray();

        var vehicleModels = new Dictionary<LoaiPhuongTien, string[]>
        {
            { LoaiPhuongTien.Oto, new[] { "Toyota Camry", "Honda CR-V", "Mazda 3", "Hyundai SantaFe", "Mercedes E200", "VinFast Lux A" } },
            { LoaiPhuongTien.XeMay, new[] { "Honda SH", "Honda Vision", "Yamaha Exciter", "Vespa Primavera", "Air Blade" } },
            { LoaiPhuongTien.XeDap, new[] { "Giant Escape", "Thống Nhất", "Trek Marlin" } },
            { LoaiPhuongTien.XeDien, new[] { "VinFast Klara", "Pega", "VinFast Vento" } }
        };

        for (int i = 0; i < soLuongPhuongTien; i++)
        {
            var loaiId = faker.PickRandom(loaiPhuongTiens);
            var model = faker.PickRandom(vehicleModels[loaiId]);
            var status = faker.PickRandom(trangThais);
            
            var pt = new PhuongTien(
                canHoId: faker.PickRandom(canHoIds),
                tenPhuongTien: $"{model} {GenerateBienSo(faker)}",
                loaiPhuongTienId: loaiId,
                bienSo: GenerateBienSo(faker),
                mauXe: model
            );

            await context.PhuongTiens.AddAsync(pt);
            await context.SaveChangesAsync();

            // Status-based card logic
            if (status == TrangThaiPhuongTien.Approved)
            {
                pt.UpdateTrangThai(TrangThaiPhuongTien.Approved, DateTime.Now);
                pt.AddThe(GenerateUniqueMaThe(faker), DateTime.Now.AddMonths(-1));
            }
            else if (status == TrangThaiPhuongTien.Disabled)
            {
                // Must be Approved first to AddThe
                pt.UpdateTrangThai(TrangThaiPhuongTien.Approved, DateTime.Now);
                pt.AddThe(GenerateUniqueMaThe(faker), DateTime.Now.AddMonths(-2));
                
                // Then Disable to lock the card
                pt.UpdateTrangThai(TrangThaiPhuongTien.Disabled, DateTime.Now);
            }
            else if (status == TrangThaiPhuongTien.Rejected)
            {
                pt.UpdateTrangThai(TrangThaiPhuongTien.Rejected, DateTime.Now);
            }
            // PendingApproval remains as created (no card)

            await context.SaveChangesAsync();
        }

        logger.LogInformation("Finished seeding PhuongTiens.");
    }

    private static string GenerateBienSo(Faker faker)
    {
        string bienSo;
        do
        {
            bienSo = $"{faker.Random.Int(29, 31)}{faker.Random.String2(1, "ABCDEFGHJK")}-{faker.Random.Int(100, 999)}.{faker.Random.Int(10, 99)}";
        } while (!_usedBienSos.Add(bienSo));
        return bienSo;
    }

    public static string RegisterBienSo(string bienSo)
    {
        _usedBienSos.Add(bienSo);
        return bienSo;
    }

    public static string RegisterMaThe(string maThe)
    {
        _usedMaThes.Add(maThe);
        return maThe;
    }

    private static string GenerateUniqueMaThe(Faker faker, string prefix = "CARD-")
    {
        string maThe;
        do
        {
            maThe = faker.Random.Replace($"{prefix}##########");
        } while (!_usedMaThes.Add(maThe));
        return maThe;
    }
}
